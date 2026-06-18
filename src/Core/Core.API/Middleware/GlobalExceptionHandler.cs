using BuildingBlocks.Exceptions;
using Core.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Middleware
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger, IHostEnvironment _environment) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, title) = exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
                ValidationException => (StatusCodes.Status400BadRequest, "Erro de validação"),
                DomainException => (StatusCodes.Status400BadRequest, "Regra de dominio violada"),
                BadRequestException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
                InvalidOperationException => (StatusCodes.Status409Conflict, "Conflito de operação"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Acesso não autorizado"),
                _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro interno no servidor")
            };

            var problem = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Title = title,
                Status = status,
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.ContentType = "application/problem+json";
            httpContext.Response.StatusCode = status;

            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }
    }
}
