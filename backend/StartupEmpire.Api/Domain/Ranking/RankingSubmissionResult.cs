namespace StartupEmpire.Api.Domain.Ranking;

public enum RankingSubmissionStatus
{
    Accepted,
    RejectedInvalidData,
    RejectedRateLimited,
    RejectedImplausibleGrowth
}

public sealed class RankingSubmissionResult
{
    public RankingSubmissionStatus Status { get; }
    public RankingEntry? Entry { get; }
    public bool IsSuccess => Status == RankingSubmissionStatus.Accepted;

    private RankingSubmissionResult(RankingSubmissionStatus status, RankingEntry? entry)
    {
        Status = status;
        Entry = entry;
    }

    public static RankingSubmissionResult Accepted(RankingEntry entry) => new(RankingSubmissionStatus.Accepted, entry);
    public static RankingSubmissionResult Rejected(RankingSubmissionStatus status) => new(status, null);
}
