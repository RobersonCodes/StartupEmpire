using NUnit.Framework;
using StartupEmpire.Core;
using StartupEmpire.Products;
using StartupEmpire.Research;

namespace StartupEmpire.Tests.EditMode
{
    public class DevelopmentServiceTests
    {
        [Test]
        public void Develop_MovesFromPlanningToDevelopment_AndAddsProgress()
        {
            var service = new DevelopmentService(new DevelopmentConfigValues(), null);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 1000, 10, 0.08);
            var product = new ProductState(def);
            var player = new PlayerState();

            service.Develop(product, player, KnowledgeTracks.Fundamentos, cycles: 1);

            Assert.AreEqual(ProductStage.Development, product.Stage);
            Assert.IsTrue(product.DevProgress > 0);
        }

        [Test]
        public void Develop_TransitionsToTesting_WhenProgressReachesRequirement()
        {
            var service = new DevelopmentService(new DevelopmentConfigValues(), null);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 10, 10, 0.08);
            var product = new ProductState(def);
            var player = new PlayerState();

            service.Develop(product, player, KnowledgeTracks.Fundamentos, cycles: 1);

            Assert.AreEqual(ProductStage.Testing, product.Stage);
        }

        [Test]
        public void Develop_WithHigherKnowledge_ProducesFasterProgressAndFewerBugs()
        {
            var service = new DevelopmentService(new DevelopmentConfigValues(), null);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100000, 10, 0.5);

            var noviceProduct = new ProductState(def);
            var novicePlayer = new PlayerState();
            service.Develop(noviceProduct, novicePlayer, KnowledgeTracks.Fundamentos, cycles: 5);

            var expertProduct = new ProductState(def);
            var expertPlayer = new PlayerState();
            expertPlayer.AddKnowledge(KnowledgeTracks.Fundamentos, 50);
            service.Develop(expertProduct, expertPlayer, KnowledgeTracks.Fundamentos, cycles: 5);

            Assert.IsTrue(expertProduct.DevProgress > noviceProduct.DevProgress);
            Assert.IsTrue(expertProduct.BugCount <= noviceProduct.BugCount);
        }

        [Test]
        public void FixBugs_ReducesBugCount_AndImprovesStability()
        {
            var eventBus = new EventBus();
            BugFixedEvent? received = null;
            eventBus.Subscribe<BugFixedEvent>(e => received = e);
            var service = new DevelopmentService(new DevelopmentConfigValues { FixPointsPerCycle = 3 }, eventBus);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def)
            {
                BugCount = 10,
                KnownBugCount = 10,
                Stability = 0.5,
                Stage = ProductStage.Testing
            };

            service.FixBugs(product, cycles: 1);

            Assert.AreEqual(7, product.BugCount);
            Assert.AreEqual(7, product.KnownBugCount);
            Assert.IsTrue(product.Stability > 0.5);
            Assert.IsNotNull(received);
            Assert.AreEqual(3, received.Value.Count);
        }

        [Test]
        public void Launch_AppliesReputationPenalty_ForOutstandingBugs()
        {
            var eventBus = new EventBus();
            var launched = false;
            eventBus.Subscribe<ProductLaunchedEvent>(_ => launched = true);
            var service = new DevelopmentService(
                new DevelopmentConfigValues { LaunchReputationPenaltyPerOutstandingBug = 0.01 }, eventBus);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def)
            {
                BugCount = 5,
                Reputation = 0.5,
                Stage = ProductStage.Testing,
                HasBeenTested = true
            };

            var launchedSuccessfully = service.Launch(product);

            Assert.IsTrue(launchedSuccessfully);
            Assert.AreEqual(ProductStage.Launched, product.Stage);
            Assert.AreEqual(0.45, product.Reputation, 0.00001);
            Assert.IsTrue(launched);
        }

        [Test]
        public void TestForBugs_FindsBugsProportionalToEfficiency()
        {
            var service = new DevelopmentService(new DevelopmentConfigValues { TestEfficiencyPerCycle = 0.6 }, null);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def) { BugCount = 10, Stage = ProductStage.Testing };

            var found = service.TestForBugs(product, cycles: 1);

            Assert.AreEqual(6, found);
            Assert.AreEqual(6, product.KnownBugCount);
            Assert.IsTrue(product.HasBeenTested);
        }

        [Test]
        public void Launch_RejectsProductBeforeDevelopmentAndTesting()
        {
            var service = new DevelopmentService(new DevelopmentConfigValues(), null);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def);

            var launched = service.Launch(product);

            Assert.IsFalse(launched);
            Assert.AreEqual(ProductStage.Planning, product.Stage);
        }

        [Test]
        public void FixBugs_OnlyFixesBugsRevealedByTesting()
        {
            var service = new DevelopmentService(new DevelopmentConfigValues { FixPointsPerCycle = 3 }, null);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def)
            {
                BugCount = 10,
                KnownBugCount = 2,
                Stage = ProductStage.Testing
            };

            service.FixBugs(product, cycles: 1);

            Assert.AreEqual(8, product.BugCount);
            Assert.AreEqual(0, product.KnownBugCount);
        }
    }
}
