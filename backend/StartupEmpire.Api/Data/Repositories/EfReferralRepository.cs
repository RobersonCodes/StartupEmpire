using Microsoft.EntityFrameworkCore;
using StartupEmpire.Api.Domain.Referrals;

namespace StartupEmpire.Api.Data.Repositories;

public sealed class EfReferralRepository : IReferralRepository
{
    private readonly AppDbContext _db;

    public EfReferralRepository(AppDbContext db) => _db = db;

    public Task<ReferralCode?> FindCodeByOwnerAsync(string ownerPlayerId, CancellationToken ct = default) =>
        _db.ReferralCodes.FirstOrDefaultAsync(c => c.OwnerPlayerId == ownerPlayerId, ct);

    public Task<ReferralCode?> FindCodeAsync(string code, CancellationToken ct = default) =>
        _db.ReferralCodes.FirstOrDefaultAsync(c => c.Code == code, ct);

    public async Task AddCodeAsync(ReferralCode code, CancellationToken ct = default)
    {
        _db.ReferralCodes.Add(code);
        await _db.SaveChangesAsync(ct);
    }

    public Task<bool> HasInviteeRedeemedAnyCodeAsync(string inviteePlayerId, CancellationToken ct = default) =>
        _db.ReferralRedemptions.AnyAsync(r => r.InviteePlayerId == inviteePlayerId, ct);

    public Task<int> CountRedemptionsForInviterAsync(string inviterPlayerId, CancellationToken ct = default) =>
        _db.ReferralRedemptions.CountAsync(r => r.InviterPlayerId == inviterPlayerId, ct);

    public async Task AddRedemptionAsync(ReferralRedemption redemption, CancellationToken ct = default)
    {
        _db.ReferralRedemptions.Add(redemption);
        await _db.SaveChangesAsync(ct);
    }
}
