using System.Net;
using System.Net.Http.Json;
using StartupEmpire.Api.Contracts.Referrals;
using Xunit;

namespace StartupEmpire.Api.Tests.Integration;

public class ReferralEndpointsIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ReferralEndpointsIntegrationTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetOrCreateCode_ThenRedeem_GrantsRewards()
    {
        var codeResponse = await _client.PostAsJsonAsync("/api/referrals/code", new GetOrCreateReferralCodeRequest("inviter-int-1"));
        Assert.Equal(HttpStatusCode.OK, codeResponse.StatusCode);
        var code = await codeResponse.Content.ReadFromJsonAsync<ReferralCodeResponse>();

        var redeemResponse = await _client.PostAsJsonAsync("/api/referrals/redeem", new RedeemReferralRequest(code!.Code, "invitee-int-1"));
        Assert.Equal(HttpStatusCode.OK, redeemResponse.StatusCode);
        var redeem = await redeemResponse.Content.ReadFromJsonAsync<RedeemReferralResponse>();

        Assert.True(redeem!.Success);
        Assert.True(redeem.InviterRewardGems > 0);
        Assert.True(redeem.InviteeRewardGems > 0);
    }

    [Fact]
    public async Task Redeem_RejectsSelfReferral_Returns400()
    {
        var codeResponse = await _client.PostAsJsonAsync("/api/referrals/code", new GetOrCreateReferralCodeRequest("inviter-int-2"));
        var code = await codeResponse.Content.ReadFromJsonAsync<ReferralCodeResponse>();

        var redeemResponse = await _client.PostAsJsonAsync("/api/referrals/redeem", new RedeemReferralRequest(code!.Code, "inviter-int-2"));

        Assert.Equal(HttpStatusCode.BadRequest, redeemResponse.StatusCode);
    }

    [Fact]
    public async Task Redeem_RejectsUnknownCode_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/referrals/redeem", new RedeemReferralRequest("NOPE1234", "invitee-int-99"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
