namespace StartupEmpire.Api.Domain.Referrals;

public sealed class ReferralConfigValues
{
    public int CodeLength { get; init; } = 7;
    public int MaxRedemptionsPerInviter { get; init; } = 20;
    public int InviterRewardGems { get; init; } = 25;
    public int InviteeRewardGems { get; init; } = 15;
}
