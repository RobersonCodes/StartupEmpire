using NUnit.Framework;
using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Products;

namespace StartupEmpire.Tests.EditMode
{
    public class CustomerAcquisitionServiceTests
    {
        [Test]
        public void RunCycle_DoesNothing_ForProductNotLaunched()
        {
            var service = new CustomerAcquisitionService(new CustomerAcquisitionConfigValues(), null);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(System.DateTime.UtcNow), null);
            var economyState = new EconomyState(0);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def) { Stage = ProductStage.Development };

            service.RunCycle(product, economy, economyState, cycles: 1);

            Assert.AreEqual(0, product.Users);
            Assert.AreEqual(0, economyState.Cash);
        }

        [Test]
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
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(System.DateTime.UtcNow), null);
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

            Assert.AreEqual(10, product.Users);
            Assert.AreEqual(5, product.PayingCustomers);
            Assert.AreEqual(25, economyState.Cash);
        }

        [Test]
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
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(System.DateTime.UtcNow), null);
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

            Assert.AreEqual(80, product.PayingCustomers);
        }
    }
}
