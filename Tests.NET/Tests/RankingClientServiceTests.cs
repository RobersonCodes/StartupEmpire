using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Progression;
using StartupEmpire.Ranking;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class RankingClientServiceTests
    {
        [Fact]
        public async Task SubmitAsync_BuildsSubmissionFromGameState()
        {
            var fakeClient = new FakeRankingClient();
            var service = new RankingClientService(fakeClient);
            var player = new PlayerState { PlayerId = "p1", Name = "Ana" };
            var economy = new EconomyState(500) { Valuation = 1000, MonthlyRecurringRevenue = 50 };
            var state = new GameState(player, economy) { Stage = CompanyStage.Freelancer };
            state.UnlockedAchievements.Add("hello_world");

            var result = await service.SubmitAsync(state, "p1", "Ana");

            Assert.True(result);
            Assert.NotNull(fakeClient.LastSubmission);
            Assert.Equal("p1", fakeClient.LastSubmission.PlayerId);
            Assert.Equal(500, fakeClient.LastSubmission.NetWorth);
            Assert.Equal(1000, fakeClient.LastSubmission.Valuation);
            Assert.Equal((int)CompanyStage.Freelancer, fakeClient.LastSubmission.ProgressStageIndex);
            Assert.Equal(1, fakeClient.LastSubmission.AchievementCount);
        }

        [Fact]
        public async Task SubmitAsync_ReturnsFalse_WhenClientThrows()
        {
            var fakeClient = new FakeRankingClient { ShouldThrow = true };
            var service = new RankingClientService(fakeClient);
            var state = new GameState(new PlayerState(), new EconomyState(0));

            var result = await service.SubmitAsync(state, "p1", "Ana");

            Assert.False(result);
        }

        private sealed class FakeRankingClient : IRankingClient
        {
            public RankingSubmission LastSubmission;
            public bool ShouldThrow;
            public bool SubmitResult = true;

            public Task<bool> SubmitAsync(RankingSubmission submission)
            {
                if (ShouldThrow) throw new InvalidOperationException("network down");
                LastSubmission = submission;
                return Task.FromResult(SubmitResult);
            }

            public Task<IReadOnlyList<RankingEntryDto>> GetTopAsync(string metric, int limit) =>
                Task.FromResult<IReadOnlyList<RankingEntryDto>>(new List<RankingEntryDto>());

            public Task<int?> GetMyRankAsync(string playerId, string metric) => Task.FromResult<int?>(null);
        }
    }
}
