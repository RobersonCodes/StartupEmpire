using StartupEmpire.Api.Domain.Ranking;

namespace StartupEmpire.Api.Tests.TestSupport;

/// Fake em memória de IRankingRepository — permite testar RankingService sem
/// nenhum banco de dados, mesmo padrão de InMemorySaveStorage usado no cliente Unity.
public sealed class InMemoryRankingRepository : IRankingRepository
{
    private readonly Dictionary<string, RankingEntry> _byPlayerId = new();

    public Task<RankingEntry?> FindByPlayerIdAsync(string playerId, CancellationToken ct = default) =>
        Task.FromResult(_byPlayerId.TryGetValue(playerId, out var entry) ? entry : null);

    public Task UpsertAsync(RankingEntry entry, CancellationToken ct = default)
    {
        _byPlayerId[entry.PlayerId] = entry;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<RankingEntry>> GetTopAsync(RankingMetric metric, int limit, CancellationToken ct = default)
    {
        IReadOnlyList<RankingEntry> top = _byPlayerId.Values
            .OrderByDescending(e => e.GetMetricValue(metric))
            .Take(limit)
            .ToList();
        return Task.FromResult(top);
    }

    public Task<int> GetRankAsync(string playerId, RankingMetric metric, CancellationToken ct = default)
    {
        if (!_byPlayerId.TryGetValue(playerId, out var player)) return Task.FromResult(-1);

        var playerValue = player.GetMetricValue(metric);
        var higherCount = _byPlayerId.Values.Count(e => e.GetMetricValue(metric) > playerValue);
        return Task.FromResult(higherCount + 1);
    }
}
