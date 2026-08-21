using System.Collections.Generic;
using StartupEmpire.Competitors;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class CompetitorServiceTests
    {
        [Fact]
        public void RunCycle_GrowsUsersAndValuation()
        {
            var config = new CompetitorConfigValues { UserGrowthRatePerCycle = 0.1, ValuationGrowthRatePerCycle = 0.2 };
            var service = new CompetitorService(config);
            var def = new CompetitorDefinition("c", "C", initialUsers: 100, initialValuation: 1000, initialReputation: 0.5, initialQuality: 0.5);
            var competitor = new CompetitorState(def);

            service.RunCycle(new List<CompetitorState> { competitor }, cycles: 1);

            Assert.Equal(110, competitor.Users, 5);
            Assert.Equal(1200, competitor.Valuation, 5);
        }

        [Fact]
        public void RunCycle_DoesNothing_WhenCyclesIsZero()
        {
            var service = new CompetitorService(new CompetitorConfigValues());
            var def = new CompetitorDefinition("c", "C", 100, 1000, 0.5, 0.5);
            var competitor = new CompetitorState(def);

            service.RunCycle(new List<CompetitorState> { competitor }, cycles: 0);

            Assert.Equal(100, competitor.Users);
        }

        [Fact]
        public void RecomputeMarketShare_DistributesProportionally()
        {
            var service = new CompetitorService(new CompetitorConfigValues());
            var def1 = new CompetitorDefinition("c1", "C1", initialUsers: 300, initialValuation: 0, initialReputation: 0, initialQuality: 0);
            var def2 = new CompetitorDefinition("c2", "C2", initialUsers: 100, initialValuation: 0, initialReputation: 0, initialQuality: 0);
            var competitors = new List<CompetitorState> { new CompetitorState(def1), new CompetitorState(def2) };

            service.RecomputeMarketShare(competitors, playerUsers: 600);

            Assert.Equal(0.3, competitors[0].MarketShare, 5);
            Assert.Equal(0.1, competitors[1].MarketShare, 5);
        }

        [Fact]
        public void GetPlayerMarketShare_ReturnsPlayerFractionOfMarket()
        {
            var service = new CompetitorService(new CompetitorConfigValues());
            var def = new CompetitorDefinition("c", "C", initialUsers: 400, initialValuation: 0, initialReputation: 0, initialQuality: 0);
            var competitors = new List<CompetitorState> { new CompetitorState(def) };

            var share = service.GetPlayerMarketShare(competitors, playerUsers: 600);

            Assert.Equal(0.6, share, 5);
        }
    }
}
