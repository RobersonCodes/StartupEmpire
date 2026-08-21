namespace StartupEmpire.Api.Domain.Referrals;

public enum ReferralRedemptionStatus
{
    Success,
    RejectedCodeNotFound,
    RejectedSelfReferral,
    RejectedAlreadyRedeemed,
    RejectedInviterLimitReached
}

public sealed class ReferralRedemptionResult
{
    public ReferralRedemptionStatus Status { get; }
    public int InviterRewardGems { get; }
    public int InviteeRewardGems { get; }
    public bool IsSuccess => Status == ReferralRedemptionStatus.Success;

    private ReferralRedemptionResult(ReferralRedemptionStatus status, int inviterReward, int inviteeReward)
    {
        Status = status;
        InviterRewardGems = inviterReward;
        InviteeRewardGems = inviteeReward;
    }

    public static ReferralRedemptionResult Success(int inviterReward, int inviteeReward) =>
        new(ReferralRedemptionStatus.Success, inviterReward, inviteeReward);

    public static ReferralRedemptionResult Rejected(ReferralRedemptionStatus status) => new(status, 0, 0);
}
