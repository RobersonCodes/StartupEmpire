using StartupEmpire.Api.Domain.Referrals;
using StartupEmpire.Api.Tests.TestSupport;
using Xunit;

namespace StartupEmpire.Api.Tests.Unit;

public class ReferralServiceTests
{
    private static ReferralService CreateService(InMemoryReferralRepository repository, ReferralConfigValues? config = null, Random? random = null) =>
        new(repository, new FakeClock(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)), config ?? new ReferralConfigValues(), random);

    [Fact]
    public async Task GetOrCreateCodeAsync_CreatesNewCode_ForNewPlayer()
    {
        var service = CreateService(new InMemoryReferralRepository());

        var code = await service.GetOrCreateCodeAsync("inviter1");

        Assert.Equal("inviter1", code.OwnerPlayerId);
        Assert.NotEmpty(code.Code);
    }

    [Fact]
    public async Task GetOrCreateCodeAsync_ReturnsSameCode_OnSecondCall()
    {
        var service = CreateService(new InMemoryReferralRepository());

        var first = await service.GetOrCreateCodeAsync("inviter1");
        var second = await service.GetOrCreateCodeAsync("inviter1");

        Assert.Equal(first.Code, second.Code);
    }

    [Fact]
    public async Task RedeemAsync_RejectsCodeNotFound()
    {
        var service = CreateService(new InMemoryReferralRepository());

        var result = await service.RedeemAsync("DOESNOTEXIST", "invitee1");

        Assert.Equal(ReferralRedemptionStatus.RejectedCodeNotFound, result.Status);
    }

    [Fact]
    public async Task RedeemAsync_RejectsSelfReferral()
    {
        var service = CreateService(new InMemoryReferralRepository());
        var code = await service.GetOrCreateCodeAsync("inviter1");

        var result = await service.RedeemAsync(code.Code, "inviter1");

        Assert.Equal(ReferralRedemptionStatus.RejectedSelfReferral, result.Status);
    }

    [Fact]
    public async Task RedeemAsync_Success_ReturnsConfiguredRewards()
    {
        var config = new ReferralConfigValues { InviterRewardGems = 25, InviteeRewardGems = 15 };
        var service = CreateService(new InMemoryReferralRepository(), config);
        var code = await service.GetOrCreateCodeAsync("inviter1");

        var result = await service.RedeemAsync(code.Code, "invitee1");

        Assert.True(result.IsSuccess);
        Assert.Equal(25, result.InviterRewardGems);
        Assert.Equal(15, result.InviteeRewardGems);
    }

    [Fact]
    public async Task RedeemAsync_RejectsSecondRedemption_BySameInvitee()
    {
        var repository = new InMemoryReferralRepository();
        var service = CreateService(repository);
        var codeA = await service.GetOrCreateCodeAsync("inviterA");
        var codeB = await service.GetOrCreateCodeAsync("inviterB");

        var first = await service.RedeemAsync(codeA.Code, "invitee1");
        var second = await service.RedeemAsync(codeB.Code, "invitee1");

        Assert.True(first.IsSuccess);
        Assert.Equal(ReferralRedemptionStatus.RejectedAlreadyRedeemed, second.Status);
    }

    [Fact]
    public async Task RedeemAsync_RejectsWhenInviterLimitReached()
    {
        var config = new ReferralConfigValues { MaxRedemptionsPerInviter = 2 };
        var service = CreateService(new InMemoryReferralRepository(), config);
        var code = await service.GetOrCreateCodeAsync("inviter1");

        var first = await service.RedeemAsync(code.Code, "invitee1");
        var second = await service.RedeemAsync(code.Code, "invitee2");
        var third = await service.RedeemAsync(code.Code, "invitee3");

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.Equal(ReferralRedemptionStatus.RejectedInviterLimitReached, third.Status);
    }
}
