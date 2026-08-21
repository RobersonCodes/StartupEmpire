using StartupEmpire.Api.Domain.Common;

namespace StartupEmpire.Api.Domain.Ranking;

/// Regras do ranking (seção 23 da missão): valida dados importantes no servidor
/// antes de aceitar uma submissão — nunca confia cegamente no cliente. O ranking
/// em si nunca bloqueia a campanha porque essas checagens vivem só aqui no backend,
/// nunca no caminho crítico do jogo offline.
public sealed class RankingService
{
    private readonly IRankingRepository _repository;
    private readonly IClock _clock;
    private readonly RankingConfigValues _config;

    public RankingService(IRankingRepository repository, IClock clock, RankingConfigValues config)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task<RankingSubmissionResult> SubmitAsync(RankingEntry submission, CancellationToken ct = default)
    {
        if (!IsValid(submission))
            return RankingSubmissionResult.Rejected(RankingSubmissionStatus.RejectedInvalidData);

        var existing = await _repository.FindByPlayerIdAsync(submission.PlayerId, ct);

        if (existing != null && _clock.UtcNow - existing.SubmittedAtUtc < _config.MinSubmissionInterval)
            return RankingSubmissionResult.Rejected(RankingSubmissionStatus.RejectedRateLimited);

        if (existing != null && IsImplausibleGrowth(existing, submission))
            return RankingSubmissionResult.Rejected(RankingSubmissionStatus.RejectedImplausibleGrowth);

        submission.Id = existing?.Id ?? Guid.NewGuid();
        submission.SubmittedAtUtc = _clock.UtcNow;

        await _repository.UpsertAsync(submission, ct);
        return RankingSubmissionResult.Accepted(submission);
    }

    public async Task<IReadOnlyList<RankingEntry>> GetTopAsync(RankingMetric metric, int limit, CancellationToken ct = default)
    {
        var clamped = Math.Clamp(limit <= 0 ? _config.DefaultTopLimit : limit, 1, _config.MaxTopLimit);
        return await _repository.GetTopAsync(metric, clamped, ct);
    }

    public Task<int> GetRankAsync(string playerId, RankingMetric metric, CancellationToken ct = default) =>
        _repository.GetRankAsync(playerId, metric, ct);

    private static bool IsValid(RankingEntry entry)
    {
        if (string.IsNullOrWhiteSpace(entry.PlayerId)) return false;
        if (string.IsNullOrWhiteSpace(entry.DisplayName)) return false;
        if (!IsFiniteNonNegative(entry.NetWorth)) return false;
        if (!IsFiniteNonNegative(entry.Valuation)) return false;
        if (!IsFiniteNonNegative(entry.MonthlyRecurringRevenue)) return false;
        if (entry.ProgressStageIndex is < 0 or > 7) return false;
        if (entry.AchievementCount < 0) return false;
        return true;
    }

    private static bool IsFiniteNonNegative(double value) => value >= 0 && !double.IsNaN(value) && !double.IsInfinity(value);

    private bool IsImplausibleGrowth(RankingEntry previous, RankingEntry incoming) =>
        ExceedsMultiple(previous.NetWorth, incoming.NetWorth)
        || ExceedsMultiple(previous.Valuation, incoming.Valuation)
        || ExceedsMultiple(previous.MonthlyRecurringRevenue, incoming.MonthlyRecurringRevenue);

    private bool ExceedsMultiple(double previous, double incoming)
    {
        if (previous <= 0) return false;
        return incoming > previous * _config.MaxPlausibleGrowthMultiple;
    }
}
