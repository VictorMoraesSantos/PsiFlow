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

        public ConsentService(IUserRepository userRepository, IConsentRepository consentRepository)
        {
            _userRepository = userRepository;
            _consentRepository = consentRepository;
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
            if (user is null) return Result.Failure(UserErrors.NotFound(userId));

            var existing = await _consentRepository.FindByUserAndVersion(userId, termsVersion.Value, privacyVersion.Value, cancellationToken);
            if (existing is not null) return Result.Failure(UserErrors.TermsNotAccepted);

            var consent = Consent.Accept(
                new UserId(userId),
                new TenantId(user.TenantId.Value),
                termsVersion,
                privacyVersion,
                dto.DocumentType,
                dto.IpAddress,
                dto.UserAgent,
                DateTime.UtcNow);
            await _consentRepository.Create(consent, cancellationToken);

            user.RecordConsent(termsVersion, privacyVersion);
            await _userRepository.Update(user, cancellationToken);
            return Result.Success();
        }
    }
}
