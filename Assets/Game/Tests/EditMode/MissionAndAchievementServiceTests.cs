using System;
using NUnit.Framework;
using StartupEmpire.Achievements;
using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Missions;
using StartupEmpire.Products;
using StartupEmpire.Progression;

namespace StartupEmpire.Tests.EditMode
{
    public class MissionAndAchievementServiceTests
    {
        [Test]
        public void EvaluateAll_CompletesMission_AndGrantsCashReward()
        {
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var missions = Chapter1Missions.Create();
            var service = new MissionService(missions, null, economy);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            state.Products.Add(new ProductState(def) { PayingCustomers = 1 });

            var completed = service.EvaluateAll(state);

            Assert.Contains("first_customer", (System.Collections.ICollection)completed);
            Assert.AreEqual(100, state.Economy.Cash);
        }

        [Test]
        public void AchievementService_UnlocksFounder_WhenStageChanges()
        {
            var eventBus = new EventBus();
            var service = new AchievementService(AchievementCatalog.Create(), eventBus);
            var state = new GameState(new PlayerState(), new EconomyState(0)) { Stage = CompanyStage.Freelancer };

            var unlocked = service.EvaluateAll(state);

            Assert.Contains("founder", (System.Collections.ICollection)unlocked);
        }
    }
}
