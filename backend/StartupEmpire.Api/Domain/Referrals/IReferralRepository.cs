namespace StartupEmpire.Api.Domain.Referrals;

public interface IReferralRepository
{
    Task<ReferralCode?> FindCodeByOwnerAsync(string ownerPlayerId, CancellationToken ct = default);
    Task<ReferralCode?> FindCodeAsync(string code, CancellationToken ct = default);
    Task AddCodeAsync(ReferralCode code, CancellationToken ct = default);
    Task<bool> HasInviteeRedeemedAnyCodeAsync(string inviteePlayerId, CancellationToken ct = default);
    Task<int> CountRedemptionsForInviterAsync(string inviterPlayerId, CancellationToken ct = default);
    Task AddRedemptionAsync(ReferralRedemption redemption, CancellationToken ct = default);
}
