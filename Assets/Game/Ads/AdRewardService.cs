using System;
using StartupEmpire.Core;
using StartupEmpire.Premium;

namespace StartupEmpire.Ads
{
    /// Ponte entre IAdService e a carteira de Gems — só concede a recompensa
    /// quando o anúncio de fato termina com sucesso (seção 22 da missão).
    public sealed class AdRewardService
    {
        private readonly IAdService _adService;
        private readonly GemWalletService _gemWallet;
        private readonly AdConfigValues _config;
        private readonly EventBus _eventBus;

        public AdRewardService(IAdService adService, GemWalletService gemWallet, AdConfigValues config, EventBus eventBus)
        {
            _adService = adService ?? throw new ArgumentNullException(nameof(adService));
            _gemWallet = gemWallet ?? throw new ArgumentNullException(nameof(gemWallet));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _eventBus = eventBus;
        }

        public bool IsRewardedAdAvailable() => _adService.IsRewardedAdAvailable();

        public void RequestRewardedGems(GemWalletState wallet, Action<AdRewardResult> onComplete)
        {
            _adService.ShowRewardedAd(result =>
            {
                if (result == AdRewardResult.Granted)
                {
                    _gemWallet.Grant(wallet, _config.RewardedAdGemAmount, GemLedgerCategory.Reward, "rewarded_ad");
                    _eventBus?.Publish(new RewardedAdCompletedEvent(_config.RewardedAdGemAmount));
                }
                onComplete?.Invoke(result);
            });
        }
    }
}
