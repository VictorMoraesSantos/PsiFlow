using BuildingBlocks.Exceptions;
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
                BadRequestException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
                InvalidOperationException => (StatusCodes.Status409Conflict, "Conflito de operação"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Acesso não autorizado"),
                _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro interno no servidor")
            };

            if (status == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Erro interno no servidor: {Message}", exception.Message);
            }
            else
            {
                _logger.LogWarning("Exceção tratada [{Status}]: {Mensagem}", status, exception.Message);
            }

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = exception.Message,
                Type = exception.GetType().Name,
                Instance = httpContext.Request.Path
            };

            problem.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problem.Extensions["exceptionType"] = exception.GetType().FullName;
            problem.Extensions["timestamp"] = DateTime.UtcNow;

            if (exception is BadRequestException badRequest && !string.IsNullOrWhiteSpace(badRequest.Details))
            {
                problem.Extensions["details"] = badRequest.Details;
            }

            if (exception is BadRequestException badRequestErrors && badRequestErrors.Errors is not null)
            {
                problem.Extensions["errors"] = badRequestErrors.Errors;
            }

            if (exception is ValidationException validationException && validationException.Errors is { Count: > 0 })
            {
                problem.Extensions["validationErrors"] = validationException.Errors;
            }

            if (_environment.IsDevelopment() && status == StatusCodes.Status500InternalServerError)
            {
                problem.Extensions["stackTrace"] = exception.StackTrace;
                if (exception.InnerException is not null)
                {
                    problem.Extensions["innerException"] = new
                    {
                        type = exception.InnerException.GetType().FullName,
                        message = exception.InnerException.Message,
                        stackTrace = exception.InnerException.StackTrace
                    };
                }
            }

            httpContext.Response.ContentType = "application/problem+json";
            httpContext.Response.StatusCode = status;

            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }
    }
}
