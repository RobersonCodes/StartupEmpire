using System.Net;
using System.Net.Http.Json;
using StartupEmpire.Api.Contracts.Ranking;
using Xunit;

namespace StartupEmpire.Api.Tests.Integration;

public class RankingEndpointsIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RankingEndpointsIntegrationTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Submit_ThenTop_ReturnsSubmittedEntry()
    {
        var request = new SubmitRankingRequest("player-int-1", "Int Test", 1000, 5000, 200, 2, 3);

        var submitResponse = await _client.PostAsJsonAsync("/api/ranking/submit", request);
        Assert.Equal(HttpStatusCode.OK, submitResponse.StatusCode);

        var top = await _client.GetFromJsonAsync<List<RankingEntryResponse>>("/api/ranking/top?metric=Valuation&limit=10");

        Assert.Contains(top!, e => e.PlayerId == "player-int-1");
    }

    [Fact]
    public async Task Submit_RejectsInvalidData_Returns400()
    {
        var request = new SubmitRankingRequest("player-int-2", "Int Test", -5, 100, 100, 1, 0);

        var response = await _client.PostAsJsonAsync("/api/ranking/submit", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetMe_ReturnsNotFound_ForUnknownPlayer()
    {
        var response = await _client.GetAsync("/api/ranking/me/does-not-exist?metric=Valuation");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMe_ReturnsRank_AfterSubmission()
    {
        await _client.PostAsJsonAsync("/api/ranking/submit", new SubmitRankingRequest("player-int-3", "Int Test", 300, 900, 50, 1, 1));

        var response = await _client.GetAsync("/api/ranking/me/player-int-3?metric=Valuation");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<RankingPositionResponse>();
        Assert.Equal("player-int-3", body!.PlayerId);
        Assert.True(body.Rank >= 1);
    }
}
