using System;

namespace StartupEmpire.Ads
{
    /// Abstração de anúncios (seção 22 da missão). A lógica de jogo nunca deve
    /// referenciar um SDK de anúncio diretamente — só esta interface. Durante o
    /// desenvolvimento, NullAdService é o adapter seguro/mock explicitamente
    /// pedido pela missão; um SDK real (AdMob, Unity Ads, etc.) entra depois como
    /// uma nova implementação substituível, sem tocar no resto do domínio.
    public interface IAdService
    {
        bool IsRewardedAdAvailable();
        void ShowRewardedAd(Action<AdRewardResult> onComplete);
        void ShowInterstitial(Action onComplete);
    }
}
