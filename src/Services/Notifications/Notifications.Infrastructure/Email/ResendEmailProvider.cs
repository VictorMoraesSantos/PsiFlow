using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Notifications.Application.Email;

namespace Notifications.Infrastructure.Email
{
    public sealed class ResendEmailProvider(ILogger<ResendEmailProvider> logger) : IEmailProvider
    {
        public string Name => "resend";

        public Task<Result<EmailDeliveryResult>> SendAsync(EmailMessage message, CancellationToken cancellationToken)
        {
            logger.LogInformation("ResendEmailProvider: enviar via Resend para {To} (subject={Subject}) ainda nao implementado.", message.To, message.Subject);
            return Task.FromResult(Result.Failure<EmailDeliveryResult>(Error.Failure("ResendEmailProvider nao configurado no MVP.")));
        }
    }
}
