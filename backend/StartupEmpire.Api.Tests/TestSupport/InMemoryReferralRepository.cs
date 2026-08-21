using StartupEmpire.Api.Domain.Referrals;

namespace StartupEmpire.Api.Tests.TestSupport;

public sealed class InMemoryReferralRepository : IReferralRepository
{
    private readonly Dictionary<string, ReferralCode> _codesByCode = new();
    private readonly Dictionary<string, ReferralCode> _codesByOwner = new();
    private readonly List<ReferralRedemption> _redemptions = new();

    public Task<ReferralCode?> FindCodeByOwnerAsync(string ownerPlayerId, CancellationToken ct = default) =>
        Task.FromResult(_codesByOwner.TryGetValue(ownerPlayerId, out var code) ? code : null);

    public Task<ReferralCode?> FindCodeAsync(string code, CancellationToken ct = default) =>
        Task.FromResult(_codesByCode.TryGetValue(code, out var found) ? found : null);

    public Task AddCodeAsync(ReferralCode code, CancellationToken ct = default)
    {
        _codesByCode[code.Code] = code;
        _codesByOwner[code.OwnerPlayerId] = code;
        return Task.CompletedTask;
    }

    public Task<bool> HasInviteeRedeemedAnyCodeAsync(string inviteePlayerId, CancellationToken ct = default) =>
        Task.FromResult(_redemptions.Any(r => r.InviteePlayerId == inviteePlayerId));

    public Task<int> CountRedemptionsForInviterAsync(string inviterPlayerId, CancellationToken ct = default) =>
        Task.FromResult(_redemptions.Count(r => r.InviterPlayerId == inviterPlayerId));

    public Task AddRedemptionAsync(ReferralRedemption redemption, CancellationToken ct = default)
    {
        _redemptions.Add(redemption);
        return Task.CompletedTask;
    }
}
