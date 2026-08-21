using System;

namespace StartupEmpire.Ads
{
    /// Implementação segura usada enquanto nenhum SDK de anúncio real está
    /// integrado — nunca trava, nunca finge sucesso.
    public sealed class NullAdService : IAdService
    {
        public bool IsRewardedAdAvailable() => false;

        public void ShowRewardedAd(Action<AdRewardResult> onComplete) => onComplete?.Invoke(AdRewardResult.NotAvailable);

        public void ShowInterstitial(Action onComplete) => onComplete?.Invoke();
    }
}
