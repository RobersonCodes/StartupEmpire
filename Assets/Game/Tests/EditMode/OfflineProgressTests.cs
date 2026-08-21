using System;
using NUnit.Framework;
using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Idle;
using StartupEmpire.Products;

namespace StartupEmpire.Tests.EditMode
{
    public class OfflineProgressTests
    {
        [Test]
        public void Calculate_ReturnsZero_WhenElapsedIsZero()
        {
            var calculator = new OfflineProgressCalculator(new EconomyConfigValues());
            var state = new GameState(new PlayerState(), new EconomyState(0));

            var summary = calculator.Calculate(state, TimeSpan.Zero);

            Assert.AreEqual(0, summary.CashEarned);
            Assert.AreEqual(TimeSpan.Zero, summary.ElapsedApplied);
        }

        [Test]
        public void Calculate_CapsElapsedAtMaxOfflineHours()
        {
            var config = new EconomyConfigValues { MaxOfflineHours = 2 };
            var calculator = new OfflineProgressCalculator(config);
            var state = new GameState(new PlayerState(), new EconomyState(0));

            var summary = calculator.Calculate(state, TimeSpan.FromHours(100));

            Assert.AreEqual(2, summary.ElapsedApplied.TotalHours);
        }

        [Test]
        public void Calculate_EarnsCash_ForLaunchedProductWithPayingCustomers()
        {
            var config = new EconomyConfigValues { MaxOfflineHours = 24, OfflineEarningsEfficiency = 0.5 };
            var calculator = new OfflineProgressCalculator(config);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def) { Stage = ProductStage.Launched, PayingCustomers = 10, Price = 2, Stability = 1 };
            state.Products.Add(product);

            var summary = calculator.Calculate(state, TimeSpan.FromHours(4));

            Assert.AreEqual(40, summary.CashEarned, 0.00001);
            Assert.AreEqual(0, summary.BugsAccumulated);
        }
    }
}
