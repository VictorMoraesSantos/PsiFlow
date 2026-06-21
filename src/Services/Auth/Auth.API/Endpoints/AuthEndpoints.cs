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
using Auth.Application.Features.Auth.Commands.RequestEmailVerification;
using Auth.Application.Features.Auth.Commands.ResetPassword;
using Auth.Application.Features.Auth.Commands.VerifyEmail;
using Auth.Application.Features.Auth.Queries.Me;
using Auth.Domain.Entities;
using BuildingBlocks.CQRS.Sender;
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
                var command = new RegisterCommand(request);
                var result = await sender.Send(command, ct);
                var response = Results.Created($"/v1/auth/users/{result.Value!.UserId}", result.Value);
                return response;
            }).AllowAnonymous().RequireRateLimiting("auth-sensitive");

            group.MapPost("/login", async (LoginDTO request, ISender sender, CancellationToken ct) =>
            {
                var command = new LoginCommand(request);
                var result = await sender.Send(command, ct);
                var response = Results.Ok(result.Value);
                return response;
            }).AllowAnonymous().RequireRateLimiting("auth-sensitive");

            group.MapPost("/refresh", async (RefreshDTO request, ISender sender, CancellationToken ct) =>
            {
                var command = new RefreshCommand(request.RefreshToken);
                var result = await sender.Send(command, ct);
                var response = Results.Ok(result.Value);
                return response;
            }).AllowAnonymous().RequireRateLimiting("auth-sensitive");

            group.MapPost("/logout", async (HttpContext http, ISender sender, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId))
                    return Results.Unauthorized();

                var command = new LogoutCommand(userId);
                var result = await sender.Send(command, ct);
                var response = Results.NoContent();
                return response;
            }).RequireAuthorization(Permissions.Auth.SessionLogout);

            group.MapGet("/me", async (HttpContext http, ISender sender, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId))
                    return Results.Unauthorized();

                var query = new MeQuery(userId);
                var result = await sender.Send(query, ct);
                var response = Results.Ok(result.Value);
                return response;
            }).RequireAuthorization(Permissions.Auth.MeRead);

            group.MapPost("/consent", async (ConsentDTO request, HttpContext http, ISender sender, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId))
                    return Results.Unauthorized();

                var payload = request with { IpAddress = http.Connection.RemoteIpAddress?.ToString(), UserAgent = http.Request.Headers.UserAgent.ToString() };
                var command = new RecordConsentCommand(userId, payload);
                var result = await sender.Send(command, ct);
                var response = Results.NoContent();
                return response;
            }).RequireAuthorization(Permissions.Auth.ConsentAccept);

            group.MapPost("/change-password", async (ChangePasswordDTO request, HttpContext http, ISender sender, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId))
                    return Results.Unauthorized();

                var command = new ChangePasswordCommand(userId, request);
                var result = await sender.Send(command, ct);
                var response = Results.NoContent();
                return response;
            }).RequireAuthorization(Permissions.Auth.PasswordChange).RequireRateLimiting("auth-sensitive");

            group.MapPost("/forgot-password", async (ForgotPasswordDTO request, ISender sender, CancellationToken ct) =>
            {
                var command = new ForgotPasswordCommand(request);
                var result = await sender.Send(command, ct);
                var response = Results.NoContent();
                return response;
            }).AllowAnonymous().RequireRateLimiting("auth-sensitive");

            group.MapPost("/reset-password", async (ResetPasswordDTO request, ISender sender, CancellationToken ct) =>
            {
                var command = new ResetPasswordCommand(request);
                var result = await sender.Send(command, ct);
                var response = Results.NoContent();
                return response;
            }).AllowAnonymous().RequireRateLimiting("auth-sensitive");

            group.MapPost("/request-email-verification", async (EmailVerificationRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new RequestEmailVerificationCommand(request.Email);
                var result = await sender.Send(command, ct);
                var response = Results.Ok(new { token = result.Value });
                return response;
            }).AllowAnonymous().RequireRateLimiting("auth-sensitive");

            group.MapPost("/verify-email", async (EmailVerificationConfirm request, ISender sender, CancellationToken ct) =>
            {
                var command = new VerifyEmailCommand(request.Email, request.Token);
                var result = await sender.Send(command, ct);
                var response = Results.NoContent();
                return response;
            }).AllowAnonymous().RequireRateLimiting("auth-sensitive");

            var users = app.MapGroup("/v1/users").WithTags("Users");

            users.MapPut("/me", async (UpdateCurrentUserProfileDTO request, HttpContext http, IUserService userService, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId))
                    return Results.Unauthorized();

                var result = await userService.UpdateCurrentUserProfileAsync(userId, request, ct);
                var response = Results.NoContent();
                return response;
            }).RequireAuthorization(Roles.RequireAuthenticated);

            users.MapGet("/{userId:int}/permissions", async (int userId, UserManager<User> userManager) =>
            {
                var user = await userManager.FindByIdAsync(userId.ToString());
                if (user is null)
                    return Results.NotFound();

                var roles = await userManager.GetRolesAsync(user);
                var claims = await userManager.GetClaimsAsync(user);
                var permissions = claims
                    .Where(claim => claim.Type == "permission")
                    .Select(claim => claim.Value)
                    .OrderBy(value => value)
                    .ToArray();

                var body = new { userId, roles, permissions };
                var response = Results.Ok(body);
                return response;
            }).RequireAuthorization(Permissions.Auth.UsersRead);

            return app;
        }

        private static bool TryGetUserId(HttpContext http, out int userId)
        {
            userId = 0;
            var claim = http.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? http.User.FindFirstValue("sub");
            var parsed = int.TryParse(claim, out userId);
            return parsed;
        }
    }
}

public sealed record EmailVerificationRequest(string Email);
public sealed record EmailVerificationConfirm(string Email, string Token);
