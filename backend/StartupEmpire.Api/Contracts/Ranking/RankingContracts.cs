using StartupEmpire.Api.Domain.Ranking;

namespace StartupEmpire.Api.Contracts.Ranking;

public sealed record SubmitRankingRequest(
    string PlayerId,
    string DisplayName,
    double NetWorth,
    double Valuation,
    double MonthlyRecurringRevenue,
    int ProgressStageIndex,
    int AchievementCount);

public sealed record RankingEntryResponse(
    string PlayerId,
    string DisplayName,
    double NetWorth,
    double Valuation,
    double MonthlyRecurringRevenue,
    int ProgressStageIndex,
    int AchievementCount,
    DateTime SubmittedAtUtc)
{
    public static RankingEntryResponse From(RankingEntry entry) => new(
        entry.PlayerId, entry.DisplayName, entry.NetWorth, entry.Valuation, entry.MonthlyRecurringRevenue,
        entry.ProgressStageIndex, entry.AchievementCount, entry.SubmittedAtUtc);
}

public sealed record SubmitRankingResponse(bool Accepted, string Status, RankingEntryResponse? Entry);

public sealed record RankingPositionResponse(string PlayerId, string Metric, int Rank);
