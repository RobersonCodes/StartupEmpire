using System;
using System.Collections.Generic;
using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Premium;

namespace StartupEmpire.Store
{
    public sealed class StoreService
    {
        private readonly List<StoreItemDefinition> _catalog;
        private readonly GemWalletService _gemWallet;
        private readonly EconomyEngine _economy;
        private readonly EventBus _eventBus;

        public StoreService(List<StoreItemDefinition> catalog, GemWalletService gemWallet,
            EconomyEngine economy, EventBus eventBus)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _gemWallet = gemWallet ?? throw new ArgumentNullException(nameof(gemWallet));
            _economy = economy ?? throw new ArgumentNullException(nameof(economy));
            _eventBus = eventBus;
        }

        public IReadOnlyList<StoreItemDefinition> Catalog => _catalog;

        public StoreItemDefinition Find(string id) => _catalog.Find(i => i.Id == id);

        /// Cosméticos são posse permanente e só podem ser comprados uma vez;
        /// boosts/itens especiais são consumíveis e podem ser comprados de novo.
        public bool TryPurchase(GameState state, StoreItemDefinition item)
        {
            if (item.Category == StoreItemCategory.Cosmetic && state.Store.PurchasedCosmeticIds.Contains(item.Id))
                return false;

            if (!_gemWallet.TrySpend(state.GemWallet, item.GemCost, $"store:{item.Id}"))
                return false;

            ApplyEffect(state, item);
            _eventBus?.Publish(new StoreItemPurchasedEvent(item.Id));
            return true;
        }

        private void ApplyEffect(GameState state, StoreItemDefinition item)
        {
            switch (item.EffectType)
            {
                case StoreItemEffectType.InstantCashBonus:
                    _economy.Earn(state.Economy, item.EffectMagnitude, $"store:{item.Id}");
                    break;

                case StoreItemEffectType.CosmeticOnly:
                    state.Store.PurchasedCosmeticIds.Add(item.Id);
                    break;

                case StoreItemEffectType.DevSpeedBoost:
                case StoreItemEffectType.AcquisitionBoost:
                    state.Store.ActiveBoosts.Add(
                        new ActiveBoost(item.Id, item.EffectType, item.EffectMagnitude, item.DurationCycles));
                    break;
            }
        }

        public void TickBoosts(StoreState store, int cycles)
        {
            if (cycles <= 0) return;
            for (var i = store.ActiveBoosts.Count - 1; i >= 0; i--)
            {
                var boost = store.ActiveBoosts[i];
                boost.RemainingCycles -= cycles;
                if (boost.RemainingCycles <= 0) store.ActiveBoosts.RemoveAt(i);
            }
        }

        public double GetBoostMultiplier(StoreState store, StoreItemEffectType effectType)
        {
            double total = 0;
            foreach (var boost in store.ActiveBoosts)
            {
                if (boost.EffectType == effectType) total += boost.Magnitude;
            }
            return 1.0 + total;
        }
    }
}
