using StartupEmpire.Core;
using StartupEmpire.Products;
using StartupEmpire.Research;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class DevelopmentServiceTests
    {
        [Fact]
        public void Develop_MovesFromPlanningToDevelopment_AndAddsProgress()
        {
            var service = new DevelopmentService(new DevelopmentConfigValues(), null);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 1000, 10, 0.08);
            var product = new ProductState(def);
            var player = new PlayerState();

            service.Develop(product, player, KnowledgeTracks.Fundamentos, cycles: 1);

            Assert.Equal(ProductStage.Development, product.Stage);
            Assert.True(product.DevProgress > 0);
        }

        [Fact]
        public void Develop_TransitionsToTesting_WhenProgressReachesRequirement()
        {
            var service = new DevelopmentService(new DevelopmentConfigValues(), null);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 10, 10, 0.08);
            var product = new ProductState(def);
            var player = new PlayerState();

            service.Develop(product, player, KnowledgeTracks.Fundamentos, cycles: 1);

            Assert.Equal(ProductStage.Testing, product.Stage);
        }

        [Fact]
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

            Assert.True(expertProduct.DevProgress > noviceProduct.DevProgress);
            Assert.True(expertProduct.BugCount <= noviceProduct.BugCount);
        }

        [Fact]
        public void FixBugs_ReducesBugCount_AndImprovesStability()
        {
            var eventBus = new EventBus();
            BugFixedEvent? received = null;
            eventBus.Subscribe<BugFixedEvent>(e => received = e);
            var service = new DevelopmentService(new DevelopmentConfigValues { FixPointsPerCycle = 3 }, eventBus);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def) { BugCount = 10, Stability = 0.5 };

            service.FixBugs(product, cycles: 1);

            Assert.Equal(7, product.BugCount);
            Assert.True(product.Stability > 0.5);
            Assert.NotNull(received);
            Assert.Equal(3, received.Value.Count);
        }

        [Fact]
        public void Launch_AppliesReputationPenalty_ForOutstandingBugs()
        {
            var eventBus = new EventBus();
            var launched = false;
            eventBus.Subscribe<ProductLaunchedEvent>(_ => launched = true);
            var service = new DevelopmentService(
                new DevelopmentConfigValues { LaunchReputationPenaltyPerOutstandingBug = 0.01 }, eventBus);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def) { BugCount = 5, Reputation = 0.5, Stage = ProductStage.Testing };

            service.Launch(product);

            Assert.Equal(ProductStage.Launched, product.Stage);
            Assert.Equal(0.45, product.Reputation, 5);
            Assert.True(launched);
        }

        [Fact]
        public void TestForBugs_FindsBugsProportionalToEfficiency()
        {
            var service = new DevelopmentService(new DevelopmentConfigValues { TestEfficiencyPerCycle = 0.6 }, null);
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            var product = new ProductState(def) { BugCount = 10, Stage = ProductStage.Testing };

            var found = service.TestForBugs(product, cycles: 1);

            Assert.Equal(6, found);
        }
    }
}
