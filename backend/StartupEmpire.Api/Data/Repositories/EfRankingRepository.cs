using Microsoft.EntityFrameworkCore;
using StartupEmpire.Api.Domain.Ranking;

namespace StartupEmpire.Api.Data.Repositories;

public sealed class EfRankingRepository : IRankingRepository
{
    private readonly AppDbContext _db;

    public EfRankingRepository(AppDbContext db) => _db = db;

    public Task<RankingEntry?> FindByPlayerIdAsync(string playerId, CancellationToken ct = default) =>
        _db.RankingEntries.FirstOrDefaultAsync(e => e.PlayerId == playerId, ct);

    public async Task UpsertAsync(RankingEntry entry, CancellationToken ct = default)
    {
        var existing = await _db.RankingEntries.FirstOrDefaultAsync(e => e.PlayerId == entry.PlayerId, ct);
        if (existing == null)
        {
            _db.RankingEntries.Add(entry);
        }
        else
        {
            existing.DisplayName = entry.DisplayName;
            existing.NetWorth = entry.NetWorth;
            existing.Valuation = entry.Valuation;
            existing.MonthlyRecurringRevenue = entry.MonthlyRecurringRevenue;
            existing.ProgressStageIndex = entry.ProgressStageIndex;
            existing.AchievementCount = entry.AchievementCount;
            existing.SubmittedAtUtc = entry.SubmittedAtUtc;
            entry.Id = existing.Id;
        }
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<RankingEntry>> GetTopAsync(RankingMetric metric, int limit, CancellationToken ct = default)
    {
        var query = OrderByMetric(_db.RankingEntries, metric);
        return await query.Take(limit).ToListAsync(ct);
    }

    public async Task<int> GetRankAsync(string playerId, RankingMetric metric, CancellationToken ct = default)
    {
        var player = await FindByPlayerIdAsync(playerId, ct);
        if (player == null) return -1;

        var playerValue = player.GetMetricValue(metric);
        var higherCount = metric switch
        {
            RankingMetric.NetWorth => await _db.RankingEntries.CountAsync(e => e.NetWorth > playerValue, ct),
            RankingMetric.Valuation => await _db.RankingEntries.CountAsync(e => e.Valuation > playerValue, ct),
            RankingMetric.MonthlyRecurringRevenue => await _db.RankingEntries.CountAsync(e => e.MonthlyRecurringRevenue > playerValue, ct),
            RankingMetric.Progress => await _db.RankingEntries.CountAsync(e => e.ProgressStageIndex > playerValue, ct),
            RankingMetric.Achievements => await _db.RankingEntries.CountAsync(e => e.AchievementCount > playerValue, ct),
            _ => 0
        };

        return higherCount + 1;
    }

    private static IQueryable<RankingEntry> OrderByMetric(IQueryable<RankingEntry> query, RankingMetric metric) => metric switch
    {
        RankingMetric.NetWorth => query.OrderByDescending(e => e.NetWorth),
        RankingMetric.Valuation => query.OrderByDescending(e => e.Valuation),
        RankingMetric.MonthlyRecurringRevenue => query.OrderByDescending(e => e.MonthlyRecurringRevenue),
        RankingMetric.Progress => query.OrderByDescending(e => e.ProgressStageIndex),
        RankingMetric.Achievements => query.OrderByDescending(e => e.AchievementCount),
        _ => query
    };
}
