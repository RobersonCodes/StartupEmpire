using System;
using StartupEmpire.Ads;
using StartupEmpire.Core;
using StartupEmpire.Domain.Tests.TestSupport;
using StartupEmpire.Premium;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class AdRewardServiceTests
    {
        [Fact]
        public void NullAdService_ReportsUnavailable_AndGrantsNothing()
        {
            var gemService = new GemWalletService(new FakeClock(DateTime.UtcNow), null);
            var service = new AdRewardService(new NullAdService(), gemService, new AdConfigValues(), null);
            var wallet = new GemWalletState();

            Assert.False(service.IsRewardedAdAvailable());

            AdRewardResult? received = null;
            service.RequestRewardedGems(wallet, result => received = result);

            Assert.Equal(AdRewardResult.NotAvailable, received);
            Assert.Equal(0, wallet.Balance);
        }

        [Fact]
        public void RequestRewardedGems_GrantsConfiguredAmount_WhenAdGranted()
        {
            var eventBus = new EventBus();
            RewardedAdCompletedEvent? published = null;
            eventBus.Subscribe<RewardedAdCompletedEvent>(e => published = e);

            var gemService = new GemWalletService(new FakeClock(DateTime.UtcNow), null);
            var adService = new FakeAdService { Result = AdRewardResult.Granted };
            var service = new AdRewardService(adService, gemService, new AdConfigValues { RewardedAdGemAmount = 10 }, eventBus);
            var wallet = new GemWalletState();

            AdRewardResult? received = null;
            service.RequestRewardedGems(wallet, result => received = result);

            Assert.Equal(AdRewardResult.Granted, received);
            Assert.Equal(10, wallet.Balance);
            Assert.NotNull(published);
            Assert.Equal(10, published.Value.GemsGranted);
        }

        [Theory]
        [InlineData(AdRewardResult.Skipped)]
        [InlineData(AdRewardResult.Failed)]
        [InlineData(AdRewardResult.NotAvailable)]
        public void RequestRewardedGems_GrantsNothing_WhenAdNotGranted(AdRewardResult result)
        {
            var gemService = new GemWalletService(new FakeClock(DateTime.UtcNow), null);
            var adService = new FakeAdService { Result = result };
            var service = new AdRewardService(adService, gemService, new AdConfigValues { RewardedAdGemAmount = 10 }, null);
            var wallet = new GemWalletState();

            service.RequestRewardedGems(wallet, _ => { });

            Assert.Equal(0, wallet.Balance);
        }

        private sealed class FakeAdService : IAdService
        {
            public AdRewardResult Result;
            public bool RewardedAdAvailable = true;

            public bool IsRewardedAdAvailable() => RewardedAdAvailable;

            public void ShowRewardedAd(Action<AdRewardResult> onComplete) => onComplete?.Invoke(Result);

            public void ShowInterstitial(Action onComplete) => onComplete?.Invoke();
        }
    }
}
