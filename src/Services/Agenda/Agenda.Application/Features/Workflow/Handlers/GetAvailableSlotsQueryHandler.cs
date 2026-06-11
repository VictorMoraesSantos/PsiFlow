using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;

namespace Agenda.Application.Features.Workflow;

public sealed class GetAvailableSlotsQueryHandler(IAgendaService service, IValidator<GetAvailableSlotsQuery> validator) : IQueryHandler<GetAvailableSlotsQuery, IReadOnlyCollection<AvailableSlotResult>>
{
    public async Task<Result<IReadOnlyCollection<AvailableSlotResult>>> Handle(GetAvailableSlotsQuery query, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(query, cancellationToken);
        return validation.IsValid
            ? await service.GetAvailableSlotsAsync(query.Request, query.TenantId, cancellationToken)
            : Result.Failure<IReadOnlyCollection<AvailableSlotResult>>(Error.Failure(string.Join("; ", validation.Errors.Select(error => error.ErrorMessage))));
    }
}
