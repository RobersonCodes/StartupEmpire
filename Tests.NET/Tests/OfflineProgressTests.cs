using System;
using StartupEmpire.Core;
using StartupEmpire.Domain.Tests.TestSupport;
using StartupEmpire.Economy;
using StartupEmpire.Idle;
using StartupEmpire.Products;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class OfflineProgressTests
    {
        [Fact]
        public void Calculate_ReturnsZero_WhenElapsedIsZero()
        {
            var calculator = new OfflineProgressCalculator(new EconomyConfigValues());
            var state = new GameState(new PlayerState(), new EconomyState(0));

            var summary = calculator.Calculate(state, TimeSpan.Zero);

            Assert.Equal(0, summary.CashEarned);
            Assert.Equal(TimeSpan.Zero, summary.ElapsedApplied);
        }

        [Fact]
        public void Calculate_CapsElapsedAtMaxOfflineHours()
        {
            var config = new EconomyConfigValues { MaxOfflineHours = 2 };
            var calculator = new OfflineProgressCalculator(config);
            var state = new GameState(new PlayerState(), new EconomyState(0));

            var summary = calculator.Calculate(state, TimeSpan.FromHours(100));

            Assert.Equal(2, summary.ElapsedApplied.TotalHours);
        }

        [Fact]
        public void Calculate_EarnsCash_ForLaunchedProductWithPayingCustomers()
        {
            var config = new EconomyConfigValues { MaxOfflineHours = 24, OfflineEarningsEfficiency = 0.5 };
            var calculator = new OfflineProgressCalculator(config);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def) { Stage = ProductStage.Launched, PayingCustomers = 10, Price = 2, Stability = 1 };
            state.Products.Add(product);

            var summary = calculator.Calculate(state, TimeSpan.FromHours(4));

            Assert.Equal(40, summary.CashEarned, 5);
            Assert.Equal(40, state.Economy.Cash, 5);
            Assert.Equal(0, summary.BugsAccumulated);
        }

        [Fact]
        public void Calculate_DoesNotEarnCash_ForUnlaunchedProduct()
        {
            var config = new EconomyConfigValues { MaxOfflineHours = 24 };
            var calculator = new OfflineProgressCalculator(config);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def) { Stage = ProductStage.Development, PayingCustomers = 10, Price = 2 };
            state.Products.Add(product);

            var summary = calculator.Calculate(state, TimeSpan.FromHours(4));

            Assert.Equal(0, summary.CashEarned);
        }

        [Fact]
        public void ApplyOfflineProgress_UpdatesLastSavedUtc_AndPublishesEvent()
        {
            var clock = new FakeClock(new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc));
            var eventBus = new EventBus();
            OfflineProgressAppliedEvent? received = null;
            eventBus.Subscribe<OfflineProgressAppliedEvent>(e => received = e);
            var calculator = new OfflineProgressCalculator(new EconomyConfigValues());
            var idle = new IdleService(calculator, clock, eventBus);
            var state = new GameState(new PlayerState(), new EconomyState(0))
            {
                LastSavedUtc = clock.UtcNow - TimeSpan.FromHours(3)
            };

            idle.ApplyOfflineProgress(state);

            Assert.Equal(clock.UtcNow, state.LastSavedUtc);
            Assert.NotNull(received);
        }
    }
}
