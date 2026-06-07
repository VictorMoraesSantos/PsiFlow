using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Middleware
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, titulo) = exception switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
                InvalidOperationException => (StatusCodes.Status409Conflict, "Conflito de operação"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
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
                Title = titulo,
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
