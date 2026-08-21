using StartupEmpire.Api.Contracts.Ranking;
using StartupEmpire.Api.Domain.Ranking;

namespace StartupEmpire.Api.Endpoints;

public static class RankingEndpoints
{
    public static void MapRankingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ranking").WithTags("Ranking");

        group.MapPost("/submit", async (SubmitRankingRequest request, RankingService service, CancellationToken ct) =>
        {
            var entry = new RankingEntry
            {
                PlayerId = request.PlayerId,
                DisplayName = request.DisplayName,
                NetWorth = request.NetWorth,
                Valuation = request.Valuation,
                MonthlyRecurringRevenue = request.MonthlyRecurringRevenue,
                ProgressStageIndex = request.ProgressStageIndex,
                AchievementCount = request.AchievementCount
            };

            var result = await service.SubmitAsync(entry, ct);
            var response = new SubmitRankingResponse(
                result.IsSuccess,
                result.Status.ToString(),
                result.Entry is null ? null : RankingEntryResponse.From(result.Entry));

            return result.Status switch
            {
                RankingSubmissionStatus.Accepted => Results.Ok(response),
                RankingSubmissionStatus.RejectedRateLimited => Results.Json(response, statusCode: StatusCodes.Status429TooManyRequests),
                _ => Results.BadRequest(response)
            };
        });

        group.MapGet("/top", async (RankingMetric metric, int? limit, RankingService service, CancellationToken ct) =>
        {
            var entries = await service.GetTopAsync(metric, limit ?? 0, ct);
            return Results.Ok(entries.Select(RankingEntryResponse.From));
        });

        group.MapGet("/me/{playerId}", async (string playerId, RankingMetric metric, RankingService service, CancellationToken ct) =>
        {
            var rank = await service.GetRankAsync(playerId, metric, ct);
            return rank <= 0
                ? Results.NotFound()
                : Results.Ok(new RankingPositionResponse(playerId, metric.ToString(), rank));
        });
    }
}
