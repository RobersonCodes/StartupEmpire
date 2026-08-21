namespace StartupEmpire.Api.Domain.Ranking;

public interface IRankingRepository
{
    Task<RankingEntry?> FindByPlayerIdAsync(string playerId, CancellationToken ct = default);
    Task UpsertAsync(RankingEntry entry, CancellationToken ct = default);
    Task<IReadOnlyList<RankingEntry>> GetTopAsync(RankingMetric metric, int limit, CancellationToken ct = default);
    Task<int> GetRankAsync(string playerId, RankingMetric metric, CancellationToken ct = default);
}
