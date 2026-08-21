using System;
using System.Collections.Generic;
using StartupEmpire.Core;
using StartupEmpire.Domain.Tests.TestSupport;
using StartupEmpire.Economy;
using StartupEmpire.Upgrades;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class UpgradeServiceTests
    {
        [Fact]
        public void GetCostForNextLevel_GrowsWithLevel()
        {
            var def = new UpgradeDefinition("u", "U", UpgradeCategory.Computer, 100, 2, 0, UpgradeEffectType.DevSpeedMultiplier, 0.1);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var service = new UpgradeService(new List<UpgradeDefinition> { def }, economy, null);
            var state = new UpgradeState();

            var costLevel0 = service.GetCostForNextLevel(def, state);
            state.SetLevel("u", 2);
            var costLevel2 = service.GetCostForNextLevel(def, state);

            Assert.Equal(100, costLevel0);
            Assert.Equal(400, costLevel2);
        }

        [Fact]
        public void TryPurchase_DeductsCash_IncrementsLevel_AndPublishesEvent()
        {
            var def = new UpgradeDefinition("u", "U", UpgradeCategory.Computer, 100, 2, 0, UpgradeEffectType.DevSpeedMultiplier, 0.1);
            var eventBus = new EventBus();
            UpgradePurchasedEvent? received = null;
            eventBus.Subscribe<UpgradePurchasedEvent>(e => received = e);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var service = new UpgradeService(new List<UpgradeDefinition> { def }, economy, eventBus);
            var upgradeState = new UpgradeState();
            var economyState = new EconomyState(150);

            var result = service.TryPurchase(def, upgradeState, economyState);

            Assert.True(result);
            Assert.Equal(50, economyState.Cash);
            Assert.Equal(1, upgradeState.GetLevel("u"));
            Assert.NotNull(received);
            Assert.Equal(1, received.Value.NewLevel);
        }

        [Fact]
        public void TryPurchase_Fails_WhenInsufficientFunds()
        {
            var def = new UpgradeDefinition("u", "U", UpgradeCategory.Computer, 100, 2, 0, UpgradeEffectType.DevSpeedMultiplier, 0.1);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var service = new UpgradeService(new List<UpgradeDefinition> { def }, economy, null);
            var upgradeState = new UpgradeState();
            var economyState = new EconomyState(10);

            var result = service.TryPurchase(def, upgradeState, economyState);

            Assert.False(result);
            Assert.Equal(0, upgradeState.GetLevel("u"));
        }

        [Fact]
        public void CanPurchase_ReturnsFalse_AtMaxLevel()
        {
            var def = new UpgradeDefinition("u", "U", UpgradeCategory.Computer, 100, 2, 1, UpgradeEffectType.DevSpeedMultiplier, 0.1);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var service = new UpgradeService(new List<UpgradeDefinition> { def }, economy, null);
            var upgradeState = new UpgradeState();
            upgradeState.SetLevel("u", 1);

            Assert.False(service.CanPurchase(def, upgradeState));
        }

        [Fact]
        public void GetMultiplier_AggregatesAcrossMatchingUpgrades()
        {
            var def1 = new UpgradeDefinition("u1", "U1", UpgradeCategory.Computer, 100, 2, 0, UpgradeEffectType.DevSpeedMultiplier, 0.1);
            var def2 = new UpgradeDefinition("u2", "U2", UpgradeCategory.Tools, 100, 2, 0, UpgradeEffectType.DevSpeedMultiplier, 0.05);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var service = new UpgradeService(new List<UpgradeDefinition> { def1, def2 }, economy, null);
            var state = new UpgradeState();
            state.SetLevel("u1", 2);
            state.SetLevel("u2", 1);

            var multiplier = service.GetMultiplier(UpgradeEffectType.DevSpeedMultiplier, state);

            Assert.Equal(1.25, multiplier, 5);
        }
    }
}
