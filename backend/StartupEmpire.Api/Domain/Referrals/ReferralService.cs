using StartupEmpire.Api.Domain.Common;

namespace StartupEmpire.Api.Domain.Referrals;

/// Código de indicação, vínculo inviter/invitee, recompensa, limite e prevenção
/// básica contra abuso (seção 24 da missão): sem auto-indicação, no máximo um
/// resgate por convidado na vida toda, e um teto de resgates por indicador.
public sealed class ReferralService
{
    private const string CodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    private readonly IReferralRepository _repository;
    private readonly IClock _clock;
    private readonly ReferralConfigValues _config;
    private readonly Random _random;

    public ReferralService(IReferralRepository repository, IClock clock, ReferralConfigValues config, Random? random = null)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _random = random ?? new Random();
    }

    public async Task<ReferralCode> GetOrCreateCodeAsync(string playerId, CancellationToken ct = default)
    {
        var existing = await _repository.FindCodeByOwnerAsync(playerId, ct);
        if (existing != null) return existing;

        ReferralCode code;
        do
        {
            code = new ReferralCode
            {
                Code = GenerateCode(),
                OwnerPlayerId = playerId,
                CreatedAtUtc = _clock.UtcNow
            };
        } while (await _repository.FindCodeAsync(code.Code, ct) != null);

        await _repository.AddCodeAsync(code, ct);
        return code;
    }

    public async Task<ReferralRedemptionResult> RedeemAsync(string code, string inviteePlayerId, CancellationToken ct = default)
    {
        var referralCode = await _repository.FindCodeAsync(code, ct);
        if (referralCode == null)
            return ReferralRedemptionResult.Rejected(ReferralRedemptionStatus.RejectedCodeNotFound);

        if (string.Equals(referralCode.OwnerPlayerId, inviteePlayerId, StringComparison.Ordinal))
            return ReferralRedemptionResult.Rejected(ReferralRedemptionStatus.RejectedSelfReferral);

        if (await _repository.HasInviteeRedeemedAnyCodeAsync(inviteePlayerId, ct))
            return ReferralRedemptionResult.Rejected(ReferralRedemptionStatus.RejectedAlreadyRedeemed);

        var redemptionCount = await _repository.CountRedemptionsForInviterAsync(referralCode.OwnerPlayerId, ct);
        if (redemptionCount >= _config.MaxRedemptionsPerInviter)
            return ReferralRedemptionResult.Rejected(ReferralRedemptionStatus.RejectedInviterLimitReached);

        await _repository.AddRedemptionAsync(new ReferralRedemption
        {
            Id = Guid.NewGuid(),
            Code = referralCode.Code,
            InviterPlayerId = referralCode.OwnerPlayerId,
            InviteePlayerId = inviteePlayerId,
            RedeemedAtUtc = _clock.UtcNow
        }, ct);

        return ReferralRedemptionResult.Success(_config.InviterRewardGems, _config.InviteeRewardGems);
    }

    private string GenerateCode()
    {
        Span<char> buffer = stackalloc char[_config.CodeLength];
        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = CodeAlphabet[_random.Next(CodeAlphabet.Length)];
        }
        return new string(buffer);
    }
}
