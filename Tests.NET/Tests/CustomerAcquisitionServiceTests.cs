using System;
using StartupEmpire.Domain.Tests.TestSupport;
using StartupEmpire.Economy;
using StartupEmpire.Products;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class CustomerAcquisitionServiceTests
    {
        [Fact]
        public void RunCycle_DoesNothing_ForProductNotLaunched()
        {
            var service = new CustomerAcquisitionService(new CustomerAcquisitionConfigValues(), null);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var economyState = new EconomyState(0);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def) { Stage = ProductStage.Development };

            service.RunCycle(product, economy, economyState, cycles: 1);

            Assert.Equal(0, product.Users);
            Assert.Equal(0, economyState.Cash);
        }

        [Fact]
        public void RunCycle_AcquiresUsersAndPayingCustomers_ForLaunchedProduct()
        {
            var config = new CustomerAcquisitionConfigValues
            {
                BaseAcquisitionRate = 10,
                ConversionRateToPaying = 0.5,
                ChurnRateBase = 0,
                StabilityChurnPenalty = 0
            };
            var service = new CustomerAcquisitionService(config, null);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var economyState = new EconomyState(0);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def)
            {
                Stage = ProductStage.Launched,
                Reputation = 1,
                Quality = 1,
                Price = 5
            };

            service.RunCycle(product, economy, economyState, cycles: 1);

            Assert.Equal(10, product.Users);
            Assert.Equal(5, product.PayingCustomers);
            Assert.Equal(25, economyState.Cash);
        }

        [Fact]
        public void RunCycle_AppliesChurn_ReducingPayingCustomers()
        {
            var config = new CustomerAcquisitionConfigValues
            {
                BaseAcquisitionRate = 0,
                ConversionRateToPaying = 0,
                ChurnRateBase = 0.2,
                StabilityChurnPenalty = 0
            };
            var service = new CustomerAcquisitionService(config, null);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var economyState = new EconomyState(0);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def)
            {
                Stage = ProductStage.Launched,
                PayingCustomers = 100,
                Stability = 1,
                Price = 1
            };

            service.RunCycle(product, economy, economyState, cycles: 1);

            Assert.Equal(80, product.PayingCustomers);
        }
    }
}
