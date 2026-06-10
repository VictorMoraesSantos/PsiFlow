using Agenda.Application.Contracts;
using BuildingBlocks.CQRS.Requests.Commands;
using BuildingBlocks.CQRS.Requests.Queries;

namespace Agenda.Application.Features.Workflow;

public sealed record CreateWeeklyAvailabilityCommand(WeeklyAvailabilityRequest Request, int TenantId) : ICommand<WeeklyAvailabilityResult>;
public sealed record UpdateWeeklyAvailabilityCommand(int AvailabilityId, WeeklyAvailabilityRequest Request, int TenantId) : ICommand<bool>;
public sealed record DeleteWeeklyAvailabilityCommand(int AvailabilityId, int TenantId) : ICommand<bool>;
public sealed record CreateScheduleBlockCommand(ScheduleBlockRequest Request, int TenantId, int UserId) : ICommand<ScheduleBlockResult>;
public sealed record DeleteScheduleBlockCommand(int BlockId, int TenantId) : ICommand<bool>;
public sealed record GetAvailableSlotsQuery(AvailableSlotsRequest Request, int TenantId) : IQuery<IReadOnlyCollection<AvailableSlotResult>>;
public sealed record CancelAppointmentCommand(int AppointmentId, CancelAppointmentRequest Request, int TenantId, int UserId) : ICommand<bool>;
