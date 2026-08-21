using StartupEmpire.Competitors;
using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Investment;
using StartupEmpire.Products;
using StartupEmpire.Statistics;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class StatisticsServiceTests
    {
        [Fact]
        public void BuildSnapshot_AggregatesProductUsersAndCustomers()
        {
            var service = new StatisticsService(new CompetitorService(new CompetitorConfigValues()));
            var state = new GameState(new PlayerState(), new EconomyState(500));
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            state.Products.Add(new ProductState(def) { Stage = ProductStage.Launched, Users = 100, PayingCustomers = 20 });
            state.Products.Add(new ProductState(def) { Stage = ProductStage.Development, Users = 5, PayingCustomers = 0 });

            var snapshot = service.BuildSnapshot(state);

            Assert.Equal(105, snapshot.TotalUsers);
            Assert.Equal(20, snapshot.TotalPayingCustomers);
            Assert.Equal(2, snapshot.ProductCount);
            Assert.Equal(1, snapshot.LaunchedProductCount);
        }

        [Fact]
        public void BuildSnapshot_SumsUpgradeLevelsAcrossAllUpgrades()
        {
            var service = new StatisticsService(new CompetitorService(new CompetitorConfigValues()));
            var state = new GameState(new PlayerState(), new EconomyState(0));
            state.Upgrades.SetLevel("better_computer", 3);
            state.Upgrades.SetLevel("better_internet", 2);

            var snapshot = service.BuildSnapshot(state);

            Assert.Equal(5, snapshot.UpgradesPurchasedLevels);
        }

        [Fact]
        public void BuildSnapshot_ComputesPlayerMarketShare_UsingCompetitorService()
        {
            var service = new StatisticsService(new CompetitorService(new CompetitorConfigValues()));
            var state = new GameState(new PlayerState(), new EconomyState(0));
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            state.Products.Add(new ProductState(def) { Stage = ProductStage.Launched, Users = 600 });
            var competitorDef = new CompetitorDefinition("c", "C", initialUsers: 400, initialValuation: 0, initialReputation: 0, initialQuality: 0);
            state.Competitors.Add(new CompetitorState(competitorDef));

            var snapshot = service.BuildSnapshot(state);

            Assert.Equal(0.6, snapshot.PlayerMarketShare, 5);
        }

        [Fact]
        public void BuildSnapshot_HandlesNullCompetitorService_ReturnsZeroMarketShare()
        {
            var service = new StatisticsService(null);
            var state = new GameState(new PlayerState(), new EconomyState(0));

            var snapshot = service.BuildSnapshot(state);

            Assert.Equal(0, snapshot.PlayerMarketShare);
        }

        [Fact]
        public void BuildSnapshot_CountsAchievementsMissionsGemsAndInvestmentRounds()
        {
            var service = new StatisticsService(new CompetitorService(new CompetitorConfigValues()));
            var state = new GameState(new PlayerState(), new EconomyState(0));
            state.UnlockedAchievements.Add("hello_world");
            state.UnlockedAchievements.Add("first_customer");
            state.Missions.CompletedMissionIds.Add("first_mrr");
            state.GemWallet.Balance = 42;
            state.RaisedInvestmentRounds.Add(InvestmentRoundType.Angel);

            var snapshot = service.BuildSnapshot(state);

            Assert.Equal(2, snapshot.UnlockedAchievementCount);
            Assert.Equal(1, snapshot.CompletedMissionCount);
            Assert.Equal(42, snapshot.GemBalance);
            Assert.Equal(1, snapshot.RaisedInvestmentRoundCount);
        }
    }
}
