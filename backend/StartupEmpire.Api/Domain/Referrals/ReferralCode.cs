namespace StartupEmpire.Api.Domain.Referrals;

public sealed class ReferralCode
{
    public string Code { get; set; } = string.Empty;
    public string OwnerPlayerId { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
