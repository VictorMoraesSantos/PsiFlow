using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using Auth.Application.DTOs.Users;
using Auth.Application.Features.Auth.Commands.ChangePassword;
using Auth.Application.Features.Auth.Commands.ForgotPassword;
using Auth.Application.Features.Auth.Commands.Login;
using Auth.Application.Features.Auth.Commands.Logout;
using Auth.Application.Features.Auth.Commands.RecordConsent;
using Auth.Application.Features.Auth.Commands.Refresh;
using Auth.Application.Features.Auth.Commands.Register;
using Auth.Application.Features.Auth.Commands.ResetPassword;
using Auth.Application.Features.Auth.Commands.SetupMfa;
using Auth.Application.Features.Auth.Commands.VerifyMfa;
using Auth.Application.Features.Auth.Queries.Me;
using Auth.Domain.Entities;
using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using static BuildingBlocks.Authorization.Policies;

namespace Auth.API.Endpoints
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/v1/auth").WithTags("Auth");

            group.MapPost("/register", async (RegisterDTO request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new RegisterCommand(request), ct);
                return result.IsSuccess
                    ? Results.Created($"/v1/auth/users/{result.Value!.UserId}", result.Value)
                    : ToProblem(result.Error!);
            }).AllowAnonymous();

            group.MapPost("/login", async (LoginDTO request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new LoginCommand(request), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : ToProblem(result.Error!);
            }).AllowAnonymous().RequireRateLimiting("auth-sensitive");

            group.MapPost("/refresh", async (RefreshDTO request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new RefreshCommand(request.RefreshToken), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : ToProblem(result.Error!);
            }).AllowAnonymous().RequireRateLimiting("auth-sensitive");

            group.MapPost("/logout", async (HttpContext http, ISender sender, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId)) return Results.Unauthorized();
                var result = await sender.Send(new LogoutCommand(userId), ct);
                return result.IsSuccess ? Results.NoContent() : ToProblem(result.Error!);
            }).RequireAuthorization(Roles.RequireAuthenticated);

            group.MapGet("/me", async (HttpContext http, ISender sender, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId)) return Results.Unauthorized();
                var result = await sender.Send(new MeQuery(userId), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : ToProblem(result.Error!);
            }).RequireAuthorization(Roles.RequireAuthenticated);

            group.MapPost("/consent", async (ConsentDTO request, HttpContext http, ISender sender, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId)) return Results.Unauthorized();
                var result = await sender.Send(new RecordConsentCommand(userId, request), ct);
                return result.IsSuccess ? Results.NoContent() : ToProblem(result.Error!);
            }).RequireAuthorization(Roles.RequireAuthenticated);

            group.MapPost("/change-password", async (ChangePasswordDTO request, HttpContext http, ISender sender, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId)) return Results.Unauthorized();
                var result = await sender.Send(new ChangePasswordCommand(userId, request), ct);
                return result.IsSuccess ? Results.NoContent() : ToProblem(result.Error!);
            }).RequireAuthorization(Roles.RequireAuthenticated);

            group.MapPost("/forgot-password", async (ForgotPasswordDTO request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new ForgotPasswordCommand(request), ct);
                return result.IsSuccess ? Results.NoContent() : ToProblem(result.Error!);
            }).AllowAnonymous().RequireRateLimiting("auth-sensitive");

            group.MapPost("/reset-password", async (ResetPasswordDTO request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new ResetPasswordCommand(request), ct);
                return result.IsSuccess ? Results.NoContent() : ToProblem(result.Error!);
            }).AllowAnonymous().RequireRateLimiting("auth-sensitive");

            group.MapPost("/mfa/setup", async (HttpContext http, ISender sender, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId)) return Results.Unauthorized();
                var result = await sender.Send(new SetupMfaCommand(userId), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : ToProblem(result.Error!);
            }).RequireAuthorization(Roles.RequirePsychologist);

            group.MapPost("/mfa/verify", async (MfaVerifyDTO request, HttpContext http, ISender sender, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId)) return Results.Unauthorized();
                var result = await sender.Send(new VerifyMfaCommand(userId, request), ct);
                return result.IsSuccess ? Results.NoContent() : ToProblem(result.Error!);
            }).RequireAuthorization(Roles.RequirePsychologist).RequireRateLimiting("auth-sensitive");

            var users = app.MapGroup("/v1/users").WithTags("Users");

            users.MapPut("/me", async (UpdateCurrentUserProfileDTO request, HttpContext http, IUserService userService, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId)) return Results.Unauthorized();
                var result = await userService.UpdateCurrentUserProfileAsync(userId, request, ct);
                return result.IsSuccess ? Results.NoContent() : ToProblem(result.Error!);
            }).RequireAuthorization(Roles.RequireAuthenticated);

            users.MapGet("/{userId:int}/permissions", async (int userId, UserManager<User> userManager) =>
            {
                var user = await userManager.FindByIdAsync(userId.ToString());
                if (user is null) return Results.NotFound();

                var roles = await userManager.GetRolesAsync(user);
                var permissions = (await userManager.GetClaimsAsync(user))
                    .Where(claim => claim.Type == "permission")
                    .Select(claim => claim.Value)
                    .OrderBy(value => value)
                    .ToArray();

                return Results.Ok(new { userId, roles, permissions });
            }).RequireAuthorization(Roles.RequireSaasAdmin);

            return app;
        }

        private static bool TryGetUserId(HttpContext http, out int userId)
        {
            userId = 0;
            var claim = http.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? http.User.FindFirstValue("sub");
            return int.TryParse(claim, out userId);
        }

        private static IResult ToProblem(Error error)
        {
            var status = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Failure => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
            return Results.Problem(title: "Error", detail: error.Description, statusCode: status);
        }
    }
}
