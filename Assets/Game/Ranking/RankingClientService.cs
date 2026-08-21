using System;
using System.Threading.Tasks;
using StartupEmpire.Core;

namespace StartupEmpire.Ranking
{
    /// Ponte entre o GameState local e o IRankingClient. Nunca deixa uma falha de
    /// rede se propagar para o loop de jogo (seção 23: "o ranking nunca deverá
    /// bloquear a campanha").
    public sealed class RankingClientService
    {
        private readonly IRankingClient _client;

        public RankingClientService(IRankingClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<bool> SubmitAsync(GameState state, string playerId, string displayName)
        {
            try
            {
                var submission = new RankingSubmission
                {
                    PlayerId = playerId,
                    DisplayName = displayName,
                    NetWorth = state.Economy.Cash,
                    Valuation = state.Economy.Valuation,
                    MonthlyRecurringRevenue = state.Economy.MonthlyRecurringRevenue,
                    ProgressStageIndex = (int)state.Stage,
                    AchievementCount = state.UnlockedAchievements.Count
                };
                return await _client.SubmitAsync(submission);
            }
            catch
            {
                return false;
            }
        }
    }
}
