namespace StartupEmpire.Api.Domain.Referrals;

/// Vínculo inviter/invitee (seção 24 da missão). No máximo um registro por
/// InviteePlayerId — reforçado aqui e também por índice único no banco.
public sealed class ReferralRedemption
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string InviterPlayerId { get; set; } = string.Empty;
    public string InviteePlayerId { get; set; } = string.Empty;
    public DateTime RedeemedAtUtc { get; set; }
}
