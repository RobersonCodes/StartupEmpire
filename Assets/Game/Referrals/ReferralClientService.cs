using System;
using System.Threading.Tasks;
using StartupEmpire.Premium;

namespace StartupEmpire.Referrals
{
    /// Ponte entre o backend de referrals e a carteira de Gems local. A recompensa
    /// só é creditada no cliente depois que o backend confirma o resgate — o backend
    /// é a fonte de verdade de quanto conceder (seção 24 da missão).
    public sealed class ReferralClientService
    {
        private readonly IReferralClient _client;
        private readonly GemWalletService _gemWallet;

        public ReferralClientService(IReferralClient client, GemWalletService gemWallet)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gemWallet = gemWallet ?? throw new ArgumentNullException(nameof(gemWallet));
        }

        public async Task<bool> RedeemAsync(GemWalletState wallet, string code, string inviteePlayerId)
        {
            try
            {
                var result = await _client.RedeemAsync(code, inviteePlayerId);
                if (result is not { Success: true }) return false;

                _gemWallet.Grant(wallet, result.InviteeRewardGems, GemLedgerCategory.Reward, "referral_redeemed");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
