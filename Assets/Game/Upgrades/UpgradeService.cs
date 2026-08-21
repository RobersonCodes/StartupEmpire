using System;
using System.Collections.Generic;
using StartupEmpire.Core;
using StartupEmpire.Economy;

namespace StartupEmpire.Upgrades
{
    /// Compra de upgrades e agregação dos multiplicadores que eles concedem ao
    /// resto do domínio (dev speed, taxa de bugs, aquisição, ganho de conhecimento).
    public sealed class UpgradeService
    {
        private readonly List<UpgradeDefinition> _definitions;
        private readonly EconomyEngine _economy;
        private readonly EventBus _eventBus;

        public UpgradeService(List<UpgradeDefinition> definitions, EconomyEngine economy, EventBus eventBus)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _economy = economy ?? throw new ArgumentNullException(nameof(economy));
            _eventBus = eventBus;
        }

        public IReadOnlyList<UpgradeDefinition> Definitions => _definitions;

        public UpgradeDefinition Find(string id) => _definitions.Find(d => d.Id == id);

        public double GetCostForNextLevel(UpgradeDefinition definition, UpgradeState state)
        {
            var level = state.GetLevel(definition.Id);
            return definition.BaseCost * Math.Pow(definition.CostGrowthFactor, level);
        }

        public bool CanPurchase(UpgradeDefinition definition, UpgradeState state) =>
            definition.MaxLevel <= 0 || state.GetLevel(definition.Id) < definition.MaxLevel;

        public bool TryPurchase(UpgradeDefinition definition, UpgradeState state, EconomyState economyState)
        {
            if (!CanPurchase(definition, state)) return false;

            var cost = GetCostForNextLevel(definition, state);
            if (!_economy.TrySpend(economyState, cost, LedgerCategory.Equipment, $"upgrade:{definition.Id}")) return false;

            var newLevel = state.GetLevel(definition.Id) + 1;
            state.SetLevel(definition.Id, newLevel);
            _eventBus?.Publish(new UpgradePurchasedEvent(definition.Id, newLevel));
            return true;
        }

        /// Multiplicador combinado de todos os upgrades de um mesmo efeito
        /// (1.0 = sem bônus; nunca retorna abaixo de 0).
        public double GetMultiplier(UpgradeEffectType effectType, UpgradeState state)
        {
            double total = 0;
            foreach (var definition in _definitions)
            {
                if (definition.EffectType != effectType) continue;
                total += definition.EffectPerLevel * state.GetLevel(definition.Id);
            }
            return Math.Max(0, 1.0 + total);
        }
    }
}
