using System;
using StartupEmpire.Achievements;
using StartupEmpire.Core;
using StartupEmpire.Domain.Tests.TestSupport;
using StartupEmpire.Economy;
using StartupEmpire.Missions;
using StartupEmpire.Products;
using StartupEmpire.Progression;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class MissionAndAchievementServiceTests
    {
        [Fact]
        public void EvaluateAll_CompletesMission_AndGrantsCashReward()
        {
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var missions = Chapter1Missions.Create();
            var service = new MissionService(missions, null, economy);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            state.Products.Add(new ProductState(def) { PayingCustomers = 1 });

            var completed = service.EvaluateAll(state);

            Assert.Contains("first_customer", completed);
            Assert.Equal(100, state.Economy.Cash);
            Assert.Contains("first_customer", state.Missions.CompletedMissionIds);
        }

        [Fact]
        public void EvaluateAll_DoesNotCompleteMissionTwice()
        {
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var missions = Chapter1Missions.Create();
            var service = new MissionService(missions, null, economy);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            state.Products.Add(new ProductState(def) { PayingCustomers = 1 });

            service.EvaluateAll(state);
            var secondRun = service.EvaluateAll(state);

            Assert.DoesNotContain("first_customer", secondRun);
            Assert.Equal(100, state.Economy.Cash);
        }

        [Fact]
        public void AchievementService_UnlocksFounder_WhenStageChanges()
        {
            var eventBus = new EventBus();
            var service = new AchievementService(AchievementCatalog.Create(), eventBus);
            var state = new GameState(new PlayerState(), new EconomyState(0)) { Stage = CompanyStage.Freelancer };

            var unlocked = service.EvaluateAll(state);

            Assert.Contains("founder", unlocked);
        }
    }
}
