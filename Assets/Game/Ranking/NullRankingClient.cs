using System.Collections.Generic;
using System.Threading.Tasks;

namespace StartupEmpire.Ranking
{
    /// Implementação padrão enquanto nenhum backend está configurado (ou o
    /// dispositivo está offline) — nunca falha, nunca bloqueia, só não faz nada.
    public sealed class NullRankingClient : IRankingClient
    {
        public Task<bool> SubmitAsync(RankingSubmission submission) => Task.FromResult(false);

        public Task<IReadOnlyList<RankingEntryDto>> GetTopAsync(string metric, int limit) =>
            Task.FromResult<IReadOnlyList<RankingEntryDto>>(new List<RankingEntryDto>());

        public Task<int?> GetMyRankAsync(string playerId, string metric) => Task.FromResult<int?>(null);
    }
}
