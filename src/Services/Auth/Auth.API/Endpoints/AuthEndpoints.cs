using System.Security.Claims;
using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Endpoints
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/v1/auth").WithTags("Auth");

            group.MapPost("/register", async (RegisterRequest request, IAuthService authService, CancellationToken ct) =>
            {
                var result = await authService.RegisterAsync(request, ct);
                return result.IsSuccess
                    ? Results.Created($"/v1/auth/users/{result.Value!.UserId}", result.Value)
                    : ToProblem(result.Error!);
            });

            group.MapPost("/login", async (LoginRequest request, IAuthService authService, CancellationToken ct) =>
            {
                var result = await authService.LoginAsync(request, ct);
                return result.IsSuccess ? Results.Ok(result.Value) : ToProblem(result.Error!);
            });

            group.MapPost("/refresh", async (RefreshRequest request, IAuthService authService, CancellationToken ct) =>
            {
                var result = await authService.RefreshAsync(request.RefreshToken, ct);
                return result.IsSuccess ? Results.Ok(result.Value) : ToProblem(result.Error!);
            });

            group.MapPost("/logout", [Authorize] async (HttpContext http, IAuthService authService, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId)) return Results.Unauthorized();
                var result = await authService.LogoutAsync(userId, ct);
                return result.IsSuccess ? Results.NoContent() : ToProblem(result.Error!);
            });

            group.MapGet("/me", [Authorize] async (HttpContext http, IAuthService authService, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId)) return Results.Unauthorized();
                var result = await authService.MeAsync(userId, ct);
                return result.IsSuccess ? Results.Ok(result.Value) : ToProblem(result.Error!);
            });

            group.MapPost("/consent", [Authorize] async (ConsentRequest request, HttpContext http, IAuthService authService, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId)) return Results.Unauthorized();
                var result = await authService.RecordConsentAsync(userId, request, ct);
                return result.IsSuccess ? Results.NoContent() : ToProblem(result.Error!);
            });

            group.MapPost("/change-password", [Authorize] async (ChangePasswordRequest request, HttpContext http, IAuthService authService, CancellationToken ct) =>
            {
                if (!TryGetUserId(http, out var userId)) return Results.Unauthorized();
                var result = await authService.ChangePasswordAsync(userId, request, ct);
                return result.IsSuccess ? Results.NoContent() : ToProblem(result.Error!);
            });

            group.MapPost("/forgot-password", async (ForgotPasswordRequest request, IAuthService authService, CancellationToken ct) =>
            {
                var result = await authService.ForgotPasswordAsync(request, ct);
                return result.IsSuccess ? Results.NoContent() : ToProblem(result.Error!);
            });

            group.MapPost("/reset-password", async (ResetPasswordRequest request, IAuthService authService, CancellationToken ct) =>
            {
                var result = await authService.ResetPasswordAsync(request, ct);
                return result.IsSuccess ? Results.NoContent() : ToProblem(result.Error!);
            });

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
