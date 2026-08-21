namespace StartupEmpire.Api.Contracts.Referrals;

public sealed record GetOrCreateReferralCodeRequest(string PlayerId);

public sealed record ReferralCodeResponse(string Code, string OwnerPlayerId, DateTime CreatedAtUtc);

public sealed record RedeemReferralRequest(string Code, string InviteePlayerId);

public sealed record RedeemReferralResponse(bool Success, string Status, int InviterRewardGems, int InviteeRewardGems);
