using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StartupEmpire.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace StartupEmpire.Ranking
{
    /// Implementação real via UnityWebRequest, falando com backend/StartupEmpire.Api.
    /// Só é usada quando um endpoint está configurado (ver GameRoot); NullRankingClient
    /// garante que o jogo funciona sem rede/backend.
    public sealed class HttpRankingClient : IRankingClient
    {
        private readonly string _baseUrl;

        public HttpRankingClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public async Task<bool> SubmitAsync(RankingSubmission submission)
        {
            using var request = BuildJsonPost($"{_baseUrl}/api/ranking/submit", JsonUtility.ToJson(submission));
            await UnityWebRequestAsync.SendAsync(request);
            return request.result == UnityWebRequest.Result.Success;
        }

        public async Task<IReadOnlyList<RankingEntryDto>> GetTopAsync(string metric, int limit)
        {
            using var request = UnityWebRequest.Get($"{_baseUrl}/api/ranking/top?metric={metric}&limit={limit}");
            await UnityWebRequestAsync.SendAsync(request);
            if (request.result != UnityWebRequest.Result.Success) return new List<RankingEntryDto>();

            var wrapped = "{\"items\":" + request.downloadHandler.text + "}";
            return JsonUtility.FromJson<RankingEntryDtoListWrapper>(wrapped)?.items ?? new List<RankingEntryDto>();
        }

        public async Task<int?> GetMyRankAsync(string playerId, string metric)
        {
            using var request = UnityWebRequest.Get($"{_baseUrl}/api/ranking/me/{playerId}?metric={metric}");
            await UnityWebRequestAsync.SendAsync(request);
            if (request.result != UnityWebRequest.Result.Success) return null;

            var response = JsonUtility.FromJson<RankingPositionDto>(request.downloadHandler.text);
            return response?.Rank;
        }

        private static UnityWebRequest BuildJsonPost(string url, string json)
        {
            var request = new UnityWebRequest(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json)),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            return request;
        }

        [Serializable]
        private sealed class RankingEntryDtoListWrapper
        {
            public List<RankingEntryDto> items;
        }

        [Serializable]
        private sealed class RankingPositionDto
        {
            public string PlayerId;
            public string Metric;
            public int Rank;
        }
    }
}
