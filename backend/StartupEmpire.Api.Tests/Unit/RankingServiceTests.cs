using StartupEmpire.Api.Domain.Ranking;
using StartupEmpire.Api.Tests.TestSupport;
using Xunit;

namespace StartupEmpire.Api.Tests.Unit;

public class RankingServiceTests
{
    private static RankingEntry ValidEntry(string playerId = "p1", double netWorth = 100) => new()
    {
        PlayerId = playerId,
        DisplayName = "Ana",
        NetWorth = netWorth,
        Valuation = netWorth * 10,
        MonthlyRecurringRevenue = 50,
        ProgressStageIndex = 1,
        AchievementCount = 2
    };

    [Fact]
    public async Task SubmitAsync_RejectsInvalidData_WhenNetWorthIsNegative()
    {
        var service = new RankingService(new InMemoryRankingRepository(), new FakeClock(DateTime.UtcNow), new RankingConfigValues());
        var entry = ValidEntry();
        entry.NetWorth = -1;

        var result = await service.SubmitAsync(entry);

        Assert.False(result.IsSuccess);
        Assert.Equal(RankingSubmissionStatus.RejectedInvalidData, result.Status);
    }

    [Fact]
    public async Task SubmitAsync_RejectsInvalidData_WhenPlayerIdIsBlank()
    {
        var service = new RankingService(new InMemoryRankingRepository(), new FakeClock(DateTime.UtcNow), new RankingConfigValues());
        var entry = ValidEntry(playerId: "  ");

        var result = await service.SubmitAsync(entry);

        Assert.Equal(RankingSubmissionStatus.RejectedInvalidData, result.Status);
    }

    [Fact]
    public async Task SubmitAsync_AcceptsFirstValidSubmission_AndPersistsIt()
    {
        var repository = new InMemoryRankingRepository();
        var service = new RankingService(repository, new FakeClock(DateTime.UtcNow), new RankingConfigValues());

        var result = await service.SubmitAsync(ValidEntry());

        Assert.True(result.IsSuccess);
        var stored = await repository.FindByPlayerIdAsync("p1");
        Assert.NotNull(stored);
        Assert.Equal(100, stored!.NetWorth);
    }

    [Fact]
    public async Task SubmitAsync_RejectsRateLimited_WhenResubmittingTooSoon()
    {
        var clock = new FakeClock(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var config = new RankingConfigValues { MinSubmissionInterval = TimeSpan.FromMinutes(2) };
        var service = new RankingService(new InMemoryRankingRepository(), clock, config);

        await service.SubmitAsync(ValidEntry());
        clock.Advance(TimeSpan.FromSeconds(30));
        var result = await service.SubmitAsync(ValidEntry(netWorth: 110));

        Assert.Equal(RankingSubmissionStatus.RejectedRateLimited, result.Status);
    }

    [Fact]
    public async Task SubmitAsync_AllowsUpdate_AfterMinIntervalElapsed()
    {
        var clock = new FakeClock(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var config = new RankingConfigValues { MinSubmissionInterval = TimeSpan.FromMinutes(2) };
        var repository = new InMemoryRankingRepository();
        var service = new RankingService(repository, clock, config);

        await service.SubmitAsync(ValidEntry(netWorth: 100));
        clock.Advance(TimeSpan.FromMinutes(3));
        var result = await service.SubmitAsync(ValidEntry(netWorth: 150));

        Assert.True(result.IsSuccess);
        var stored = await repository.FindByPlayerIdAsync("p1");
        Assert.Equal(150, stored!.NetWorth);
    }

    [Fact]
    public async Task SubmitAsync_RejectsImplausibleGrowth_WhenJumpExceedsConfiguredMultiple()
    {
        var clock = new FakeClock(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var config = new RankingConfigValues
        {
            MinSubmissionInterval = TimeSpan.FromMinutes(2),
            MaxPlausibleGrowthMultiple = 1000
        };
        var service = new RankingService(new InMemoryRankingRepository(), clock, config);

        await service.SubmitAsync(ValidEntry(netWorth: 100));
        clock.Advance(TimeSpan.FromMinutes(3));
        var result = await service.SubmitAsync(ValidEntry(netWorth: 100 * 1000 + 1));

        Assert.Equal(RankingSubmissionStatus.RejectedImplausibleGrowth, result.Status);
    }

    [Fact]
    public async Task GetTopAsync_ReturnsEntriesOrderedByMetricDescending()
    {
        var repository = new InMemoryRankingRepository();
        var clock = new FakeClock(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var service = new RankingService(repository, clock, new RankingConfigValues());

        await service.SubmitAsync(ValidEntry("p1", netWorth: 100));
        await service.SubmitAsync(ValidEntry("p2", netWorth: 500));
        await service.SubmitAsync(ValidEntry("p3", netWorth: 250));

        var top = await service.GetTopAsync(RankingMetric.NetWorth, limit: 10);

        Assert.Equal(new[] { "p2", "p3", "p1" }, top.Select(e => e.PlayerId));
    }

    [Fact]
    public async Task GetRankAsync_ReturnsOneBasedPosition()
    {
        var repository = new InMemoryRankingRepository();
        var clock = new FakeClock(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var service = new RankingService(repository, clock, new RankingConfigValues());

        await service.SubmitAsync(ValidEntry("p1", netWorth: 100));
        await service.SubmitAsync(ValidEntry("p2", netWorth: 500));
        await service.SubmitAsync(ValidEntry("p3", netWorth: 250));

        var rank = await service.GetRankAsync("p3", RankingMetric.NetWorth);

        Assert.Equal(2, rank);
    }
}
