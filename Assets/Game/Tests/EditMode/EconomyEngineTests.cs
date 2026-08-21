using System;
using NUnit.Framework;
using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Products;

namespace StartupEmpire.Tests.EditMode
{
    public class EconomyEngineTests
    {
        [Test]
        public void TrySpend_ReturnsFalse_WhenInsufficientFunds()
        {
            var engine = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var state = new EconomyState(startingCash: 10);

            var result = engine.TrySpend(state, 50, LedgerCategory.Marketing, "ads");

            Assert.IsFalse(result);
            Assert.AreEqual(10, state.Cash);
            Assert.IsEmpty(state.Ledger);
        }

        [Test]
        public void TrySpend_DeductsCash_WhenFundsSufficient()
        {
            var engine = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var state = new EconomyState(startingCash: 100);

            var result = engine.TrySpend(state, 40, LedgerCategory.Equipment, "laptop");

            Assert.IsTrue(result);
            Assert.AreEqual(60, state.Cash);
            Assert.AreEqual(1, state.Ledger.Count);
        }

        [Test]
        public void Earn_IncreasesCash_AndPublishesRevenueEvent()
        {
            var eventBus = new EventBus();
            RevenueEarnedEvent? received = null;
            eventBus.Subscribe<RevenueEarnedEvent>(e => received = e);
            var engine = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), eventBus);
            var state = new EconomyState(startingCash: 0);

            engine.Earn(state, 25.5, "first_website");

            Assert.AreEqual(25.5, state.Cash);
            Assert.IsNotNull(received);
            Assert.AreEqual("first_website", received.Value.SourceProductId);
        }

        [Test]
        public void RecomputeValuation_UsesConfiguredMultipliers()
        {
            var config = new EconomyConfigValues { ValuationMrrMultiple = 12, ValuationSectorMultiplier = 3 };
            var engine = new EconomyEngine(config, new FakeClock(DateTime.UtcNow), null);
            var state = new EconomyState(0) { MonthlyRecurringRevenue = 1000 };

            engine.RecomputeValuation(state);

            Assert.AreEqual(1000 * 12 * 3, state.Valuation);
        }

        [Test]
        public void RecomputeRecurringRevenue_OnlyCountsLaunchedOrMaintenanceProducts()
        {
            var engine = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var state = new EconomyState(0);
            var launchedDef = new ProductDefinition("a", "A", ProductCategory.Website, 100, 10, 0.1);
            var planningDef = new ProductDefinition("b", "B", ProductCategory.Website, 100, 20, 0.1);
            var launched = new ProductState(launchedDef) { Stage = ProductStage.Launched, PayingCustomers = 5, Price = 10 };
            var planning = new ProductState(planningDef) { Stage = ProductStage.Planning, PayingCustomers = 999, Price = 20 };

            engine.RecomputeRecurringRevenue(state, new[] { launched, planning });

            Assert.AreEqual(50, state.MonthlyRecurringRevenue);
        }
    }
}
