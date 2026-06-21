using Auth.Application.Contracts;
using Auth.Application.DTOs.Auth;
using Auth.Domain.Entities;
using Auth.Domain.Errors;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using BuildingBlocks.Results;

namespace Auth.Application.Services
{
    public class ConsentService : IConsentService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConsentRepository _consentRepository;
        private readonly IOutboxService _outboxService;

        public ConsentService(IUserRepository userRepository, IConsentRepository consentRepository, IOutboxService outboxService)
        {
            _userRepository = userRepository;
            _consentRepository = consentRepository;
            _outboxService = outboxService;
        }

        public async Task<Result> RecordAsync(int userId, ConsentDTO dto, CancellationToken cancellationToken = default)
        {
            DocumentVersion termsVersion;
            DocumentVersion privacyVersion;
            try
            {
                termsVersion = DocumentVersion.Create(dto.TermsVersion, nameof(dto.TermsVersion));
                privacyVersion = DocumentVersion.Create(dto.PrivacyVersion, nameof(dto.PrivacyVersion));
            }
            catch (Exception)
            {
                return Result.Failure(UserErrors.TermsNotAccepted);
            }

            var user = await _userRepository.GetById(new UserId(userId), cancellationToken);
            if (user is null)
                return Result.Failure(UserErrors.NotFound(userId));

            var existing = await _consentRepository.FindByUserAndVersion(userId, termsVersion.Value, privacyVersion.Value, cancellationToken);
            if (existing is not null)
                return Result.Failure(UserErrors.TermsNotAccepted);

            var consent = user.AcceptConsent(termsVersion, privacyVersion, dto.DocumentType, dto.IpAddress, dto.UserAgent);
            await _consentRepository.Create(consent, cancellationToken);

            user.LinkToConsent(consent.Id);
            await _userRepository.Update(user, cancellationToken);

            var correlationId = Guid.NewGuid();
            await _outboxService.PersistEventsAsync(consent.Id.Value, nameof(Consent), consent.DomainEvents, correlationId, cancellationToken);
            consent.ClearDomainEvents();

            return Result.Success();
        }
    }
}
