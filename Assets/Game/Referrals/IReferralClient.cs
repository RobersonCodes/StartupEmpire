using System.Threading.Tasks;

namespace StartupEmpire.Referrals
{
    public interface IReferralClient
    {
        Task<ReferralCodeDto> GetOrCreateCodeAsync(string playerId);
        Task<ReferralRedemptionResultDto> RedeemAsync(string code, string inviteePlayerId);
    }
}
