using BuildingBlocks.Results;

namespace Notifications.Application.Email
{
    public interface IEmailProvider
    {
        string Name { get; }
        Task<Result<EmailDeliveryResult>> SendAsync(EmailMessage message, CancellationToken cancellationToken);
    }

    public sealed record EmailMessage(string To, string Subject, string BodyHtml, string BodyText);

    public sealed record EmailDeliveryResult(string ProviderMessageId, string Status);
}
