using System.Threading.Tasks;

namespace StartupEmpire.Referrals
{
    /// Implementação padrão enquanto nenhum backend está configurado — resgates
    /// nunca funcionam offline (a seção 24 já prevê que este recurso depende do
    /// backend), mas o jogo nunca quebra por causa disso.
    public sealed class NullReferralClient : IReferralClient
    {
        public Task<ReferralCodeDto> GetOrCreateCodeAsync(string playerId) => Task.FromResult<ReferralCodeDto>(null);

        public Task<ReferralRedemptionResultDto> RedeemAsync(string code, string inviteePlayerId) =>
            Task.FromResult(new ReferralRedemptionResultDto { Success = false, Status = "OfflineNoBackend" });
    }
}
