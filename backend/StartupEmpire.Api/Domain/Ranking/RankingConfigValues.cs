namespace StartupEmpire.Api.Domain.Ranking;

public sealed class RankingConfigValues
{
    public TimeSpan MinSubmissionInterval { get; init; } = TimeSpan.FromMinutes(2);
    public double MaxPlausibleGrowthMultiple { get; init; } = 1000.0;
    public int DefaultTopLimit { get; init; } = 50;
    public int MaxTopLimit { get; init; } = 200;
}
