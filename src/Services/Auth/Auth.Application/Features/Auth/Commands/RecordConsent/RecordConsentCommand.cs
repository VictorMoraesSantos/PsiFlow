using Auth.Application.DTOs.Auth;
using BuildingBlocks.CQRS.Requests.Commands;

namespace Auth.Application.Features.Auth.Commands.RecordConsent;

public sealed record RecordConsentCommand(int UserId, ConsentDTO Consent) : ICommand;
