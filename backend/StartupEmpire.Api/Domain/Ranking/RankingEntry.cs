namespace StartupEmpire.Api.Domain.Ranking;

/// Uma linha do ranking global, uma por PlayerId (upsert a cada submissão).
public sealed class RankingEntry
{
    public Guid Id { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public double NetWorth { get; set; }
    public double Valuation { get; set; }
    public double MonthlyRecurringRevenue { get; set; }
    public int ProgressStageIndex { get; set; }
    public int AchievementCount { get; set; }
    public DateTime SubmittedAtUtc { get; set; }

    public double GetMetricValue(RankingMetric metric) => metric switch
    {
        RankingMetric.NetWorth => NetWorth,
        RankingMetric.Valuation => Valuation,
        RankingMetric.MonthlyRecurringRevenue => MonthlyRecurringRevenue,
        RankingMetric.Progress => ProgressStageIndex,
        RankingMetric.Achievements => AchievementCount,
        _ => 0
    };
}
