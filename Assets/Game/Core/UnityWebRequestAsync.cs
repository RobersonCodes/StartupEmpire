using System.Threading.Tasks;
using UnityEngine.Networking;

namespace StartupEmpire.Core
{
    /// Pequeno helper para usar UnityWebRequest com async/await sem depender de um
    /// pacote extra só para isso. UnityWebRequestAsyncOperation não tem GetAwaiter
    /// nativo, então isso embrulha o callback `completed` num TaskCompletionSource.
    internal static class UnityWebRequestAsync
    {
        public static Task<UnityWebRequest> SendAsync(UnityWebRequest request)
        {
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            var operation = request.SendWebRequest();
            operation.completed += _ => tcs.SetResult(request);
            return tcs.Task;
        }
    }
}
