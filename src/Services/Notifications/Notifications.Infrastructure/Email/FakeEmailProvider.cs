using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Notifications.Application.Email;

namespace Notifications.Infrastructure.Email
{
    public sealed class FakeEmailProvider(ILogger<FakeEmailProvider> logger) : IEmailProvider
    {
        public string Name => "fake";

        public Task<Result<EmailDeliveryResult>> SendAsync(EmailMessage message, CancellationToken cancellationToken)
        {
            logger.LogInformation("FakeEmailProvider: enviando e-mail para {To} com assunto {Subject}", message.To, message.Subject);
            return Task.FromResult(Result.Success(new EmailDeliveryResult(Guid.NewGuid().ToString("N"), "sent")));
        }
    }
}
