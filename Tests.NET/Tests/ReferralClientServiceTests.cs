using System;
using System.Threading.Tasks;
using StartupEmpire.Domain.Tests.TestSupport;
using StartupEmpire.Premium;
using StartupEmpire.Referrals;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class ReferralClientServiceTests
    {
        [Fact]
        public async Task RedeemAsync_GrantsGemsLocally_OnSuccess()
        {
            var fakeClient = new FakeReferralClient
            {
                Result = new ReferralRedemptionResultDto { Success = true, Status = "Success", InviterRewardGems = 25, InviteeRewardGems = 15 }
            };
            var gemService = new GemWalletService(new FakeClock(DateTime.UtcNow), null);
            var service = new ReferralClientService(fakeClient, gemService);
            var wallet = new GemWalletState();

            var result = await service.RedeemAsync(wallet, "CODE123", "invitee1");

            Assert.True(result);
            Assert.Equal(15, wallet.Balance);
        }

        [Fact]
        public async Task RedeemAsync_DoesNotGrantGems_WhenClientReturnsFailure()
        {
            var fakeClient = new FakeReferralClient
            {
                Result = new ReferralRedemptionResultDto { Success = false, Status = "RejectedSelfReferral" }
            };
            var gemService = new GemWalletService(new FakeClock(DateTime.UtcNow), null);
            var service = new ReferralClientService(fakeClient, gemService);
            var wallet = new GemWalletState();

            var result = await service.RedeemAsync(wallet, "CODE123", "invitee1");

            Assert.False(result);
            Assert.Equal(0, wallet.Balance);
        }

        [Fact]
        public async Task RedeemAsync_ReturnsFalse_WhenClientThrows()
        {
            var fakeClient = new FakeReferralClient { ShouldThrow = true };
            var gemService = new GemWalletService(new FakeClock(DateTime.UtcNow), null);
            var service = new ReferralClientService(fakeClient, gemService);
            var wallet = new GemWalletState();

            var result = await service.RedeemAsync(wallet, "CODE123", "invitee1");

            Assert.False(result);
        }

        private sealed class FakeReferralClient : IReferralClient
        {
            public ReferralRedemptionResultDto Result;
            public bool ShouldThrow;

            public Task<ReferralCodeDto> GetOrCreateCodeAsync(string playerId) => Task.FromResult<ReferralCodeDto>(null);

            public Task<ReferralRedemptionResultDto> RedeemAsync(string code, string inviteePlayerId)
            {
                if (ShouldThrow) throw new InvalidOperationException("network down");
                return Task.FromResult(Result);
            }
        }
    }
}
