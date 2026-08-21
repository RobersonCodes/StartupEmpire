using System;
using System.Collections.Generic;
using StartupEmpire.Core;
using StartupEmpire.Domain.Tests.TestSupport;
using StartupEmpire.Economy;
using StartupEmpire.Premium;
using StartupEmpire.Store;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class StoreServiceTests
    {
        [Fact]
        public void TryPurchase_Fails_WhenInsufficientGems()
        {
            var gemService = new GemWalletService(new FakeClock(DateTime.UtcNow), null);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var item = new StoreItemDefinition("i1", "Item", StoreItemCategory.Special, 50, StoreItemEffectType.InstantCashBonus, 100, 0);
            var service = new StoreService(new List<StoreItemDefinition> { item }, gemService, economy, null);
            var state = new GameState(new PlayerState(), new EconomyState(0));

            var result = service.TryPurchase(state, item);

            Assert.False(result);
        }

        [Fact]
        public void TryPurchase_InstantCashBonus_SpendsGemsAndAddsCash()
        {
            var eventBus = new EventBus();
            StoreItemPurchasedEvent? received = null;
            eventBus.Subscribe<StoreItemPurchasedEvent>(e => received = e);
            var gemService = new GemWalletService(new FakeClock(DateTime.UtcNow), eventBus);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), eventBus);
            var item = new StoreItemDefinition("cash_injection", "Aporte", StoreItemCategory.Special, 50, StoreItemEffectType.InstantCashBonus, 500, 0);
            var service = new StoreService(new List<StoreItemDefinition> { item }, gemService, economy, eventBus);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            gemService.Grant(state.GemWallet, 100, GemLedgerCategory.Reward, "seed");

            var result = service.TryPurchase(state, item);

            Assert.True(result);
            Assert.Equal(50, state.GemWallet.Balance);
            Assert.Equal(500, state.Economy.Cash);
            Assert.NotNull(received);
        }

        [Fact]
        public void TryPurchase_Cosmetic_CannotBeBoughtTwice()
        {
            var gemService = new GemWalletService(new FakeClock(DateTime.UtcNow), null);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var item = new StoreItemDefinition("cosmetic", "Tema", StoreItemCategory.Cosmetic, 30, StoreItemEffectType.CosmeticOnly, 0, 0);
            var service = new StoreService(new List<StoreItemDefinition> { item }, gemService, economy, null);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            gemService.Grant(state.GemWallet, 100, GemLedgerCategory.Reward, "seed");

            var firstPurchase = service.TryPurchase(state, item);
            var secondPurchase = service.TryPurchase(state, item);

            Assert.True(firstPurchase);
            Assert.False(secondPurchase);
            Assert.Equal(70, state.GemWallet.Balance);
            Assert.Contains("cosmetic", state.Store.PurchasedCosmeticIds);
        }

        [Fact]
        public void TryPurchase_Boost_AddsActiveBoostWithDuration()
        {
            var gemService = new GemWalletService(new FakeClock(DateTime.UtcNow), null);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var item = new StoreItemDefinition("dev_boost", "Boost", StoreItemCategory.Boost, 20, StoreItemEffectType.DevSpeedBoost, 0.5, 3);
            var service = new StoreService(new List<StoreItemDefinition> { item }, gemService, economy, null);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            gemService.Grant(state.GemWallet, 100, GemLedgerCategory.Reward, "seed");

            service.TryPurchase(state, item);

            Assert.Single(state.Store.ActiveBoosts);
            Assert.Equal(3, state.Store.ActiveBoosts[0].RemainingCycles);
        }

        [Fact]
        public void TickBoosts_DecrementsAndRemovesExpiredBoosts()
        {
            var service = new StoreService(new List<StoreItemDefinition>(),
                new GemWalletService(new FakeClock(DateTime.UtcNow), null),
                new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null), null);
            var store = new StoreState();
            store.ActiveBoosts.Add(new ActiveBoost("i1", StoreItemEffectType.DevSpeedBoost, 0.5, remainingCycles: 2));

            service.TickBoosts(store, cycles: 1);
            Assert.Single(store.ActiveBoosts);

            service.TickBoosts(store, cycles: 1);
            Assert.Empty(store.ActiveBoosts);
        }

        [Fact]
        public void GetBoostMultiplier_AggregatesActiveBoostsOfSameType()
        {
            var service = new StoreService(new List<StoreItemDefinition>(),
                new GemWalletService(new FakeClock(DateTime.UtcNow), null),
                new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null), null);
            var store = new StoreState();
            store.ActiveBoosts.Add(new ActiveBoost("i1", StoreItemEffectType.DevSpeedBoost, 0.5, 3));
            store.ActiveBoosts.Add(new ActiveBoost("i2", StoreItemEffectType.DevSpeedBoost, 0.2, 1));
            store.ActiveBoosts.Add(new ActiveBoost("i3", StoreItemEffectType.AcquisitionBoost, 0.9, 1));

            var multiplier = service.GetBoostMultiplier(store, StoreItemEffectType.DevSpeedBoost);

            Assert.Equal(1.7, multiplier, 5);
        }
    }
}
