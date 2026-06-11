using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Requests.Queries;

namespace Agenda.Application.Features.Workflow;

public sealed record GetAvailableSlotsQuery(AvailableSlotsRequest Request, int TenantId) : IQuery<IReadOnlyCollection<AvailableSlotResult>>;
