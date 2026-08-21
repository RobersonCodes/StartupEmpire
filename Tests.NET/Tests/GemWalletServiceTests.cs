using System;
using StartupEmpire.Core;
using StartupEmpire.Domain.Tests.TestSupport;
using StartupEmpire.Premium;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class GemWalletServiceTests
    {
        [Fact]
        public void Grant_IncreasesBalance_AndPublishesEvent()
        {
            var eventBus = new EventBus();
            GemsGrantedEvent? received = null;
            eventBus.Subscribe<GemsGrantedEvent>(e => received = e);
            var service = new GemWalletService(new FakeClock(DateTime.UtcNow), eventBus);
            var wallet = new GemWalletState();

            service.Grant(wallet, 50, GemLedgerCategory.Reward, "test");

            Assert.Equal(50, wallet.Balance);
            Assert.NotNull(received);
            Assert.Equal(50, received.Value.Amount);
        }

        [Fact]
        public void Grant_DoesNothing_WhenAmountIsZeroOrNegative()
        {
            var service = new GemWalletService(new FakeClock(DateTime.UtcNow), null);
            var wallet = new GemWalletState();

            service.Grant(wallet, 0, GemLedgerCategory.Reward, "test");

            Assert.Equal(0, wallet.Balance);
            Assert.Empty(wallet.Ledger);
        }

        [Fact]
        public void TrySpend_DeductsBalance_WhenAffordable()
        {
            var service = new GemWalletService(new FakeClock(DateTime.UtcNow), null);
            var wallet = new GemWalletState();
            service.Grant(wallet, 100, GemLedgerCategory.Reward, "seed");

            var result = service.TrySpend(wallet, 30, "store:item");

            Assert.True(result);
            Assert.Equal(70, wallet.Balance);
        }

        [Fact]
        public void TrySpend_Fails_WhenInsufficientBalance()
        {
            var service = new GemWalletService(new FakeClock(DateTime.UtcNow), null);
            var wallet = new GemWalletState();

            var result = service.TrySpend(wallet, 10, "store:item");

            Assert.False(result);
            Assert.Equal(0, wallet.Balance);
        }
    }
}
