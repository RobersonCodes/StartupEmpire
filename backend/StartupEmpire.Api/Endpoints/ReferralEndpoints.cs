using StartupEmpire.Api.Contracts.Referrals;
using StartupEmpire.Api.Domain.Referrals;

namespace StartupEmpire.Api.Endpoints;

public static class ReferralEndpoints
{
    public static void MapReferralEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/referrals").WithTags("Referrals");

        group.MapPost("/code", async (GetOrCreateReferralCodeRequest request, ReferralService service, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.PlayerId)) return Results.BadRequest();

            var code = await service.GetOrCreateCodeAsync(request.PlayerId, ct);
            return Results.Ok(new ReferralCodeResponse(code.Code, code.OwnerPlayerId, code.CreatedAtUtc));
        });

        group.MapPost("/redeem", async (RedeemReferralRequest request, ReferralService service, CancellationToken ct) =>
        {
            var result = await service.RedeemAsync(request.Code, request.InviteePlayerId, ct);
            var response = new RedeemReferralResponse(
                result.IsSuccess, result.Status.ToString(), result.InviterRewardGems, result.InviteeRewardGems);

            return result.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
        });
    }
}
