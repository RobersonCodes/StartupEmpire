using System;
using System.Text;
using System.Threading.Tasks;
using StartupEmpire.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace StartupEmpire.Referrals
{
    public sealed class HttpReferralClient : IReferralClient
    {
        private readonly string _baseUrl;

        public HttpReferralClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public async Task<ReferralCodeDto> GetOrCreateCodeAsync(string playerId)
        {
            var body = JsonUtility.ToJson(new GetOrCreateCodeRequestDto { PlayerId = playerId });
            using var request = BuildJsonPost($"{_baseUrl}/api/referrals/code", body);
            await UnityWebRequestAsync.SendAsync(request);
            return request.result == UnityWebRequest.Result.Success
                ? JsonUtility.FromJson<ReferralCodeDto>(request.downloadHandler.text)
                : null;
        }

        public async Task<ReferralRedemptionResultDto> RedeemAsync(string code, string inviteePlayerId)
        {
            var body = JsonUtility.ToJson(new RedeemRequestDto { Code = code, InviteePlayerId = inviteePlayerId });
            using var request = BuildJsonPost($"{_baseUrl}/api/referrals/redeem", body);
            await UnityWebRequestAsync.SendAsync(request);

            if (string.IsNullOrEmpty(request.downloadHandler?.text))
            {
                return new ReferralRedemptionResultDto { Success = false, Status = "NetworkError" };
            }
            return JsonUtility.FromJson<ReferralRedemptionResultDto>(request.downloadHandler.text);
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
        private sealed class GetOrCreateCodeRequestDto
        {
            public string PlayerId;
        }

        [Serializable]
        private sealed class RedeemRequestDto
        {
            public string Code;
            public string InviteePlayerId;
        }
    }
}
