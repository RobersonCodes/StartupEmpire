using System.Collections.Generic;
using System.Threading.Tasks;

namespace StartupEmpire.Ranking
{
    /// Abstrai a chamada HTTP ao backend de ranking (seção 23 da missão). Qualquer
    /// implementação real deve falhar graciosamente (nunca lançar) quando o backend
    /// estiver indisponível — o ranking nunca deve bloquear a campanha.
    public interface IRankingClient
    {
        Task<bool> SubmitAsync(RankingSubmission submission);
        Task<IReadOnlyList<RankingEntryDto>> GetTopAsync(string metric, int limit);
        Task<int?> GetMyRankAsync(string playerId, string metric);
    }
}
