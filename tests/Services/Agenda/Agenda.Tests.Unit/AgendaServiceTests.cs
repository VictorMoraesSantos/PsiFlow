using Agenda.Application.Contracts;
using Agenda.Application.DTOs.Appointment;
using Agenda.Application.Services;
using Agenda.Domain.Entities;
using Agenda.Domain.Filters;
using Agenda.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq.Expressions;
using Xunit;

namespace PsiFlow.Agenda.Tests.Unit;

public sealed class AgendaServiceTests
{
    [Fact]
    public async Task GetAvailableSlotsAsync_ShouldGenerateSlotsAndRemoveBlockedPeriods()
    {
        var day = NextDayOfWeek(DayOfWeek.Monday);
        var availabilityRepository = new InMemoryAvailabilityRepository(new Availability
        {
            TenantId = 1,
            Weekday = (int)day.DayOfWeek,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(10, 0),
            SlotDurationMinutes = 30,
            Modality = "online",
            IsActive = true
        });
        var blockRepository = new InMemoryScheduleBlockRepository(new ScheduleBlock
        {
            TenantId = 1,
            StartsAt = day.Date.AddHours(9).AddMinutes(30),
            EndsAt = day.Date.AddHours(10)
        });
        var service = new AgendaService(availabilityRepository, blockRepository, new InMemoryAppointmentRepository());

        var result = await service.GetAvailableSlotsAsync(new AvailableSlotsRequest(day.Date, day.Date.AddDays(1).AddTicks(-1), "online"), 1, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var slot = Assert.Single(result.Value!);
        Assert.Equal(day.Date.AddHours(9), slot.StartsAt);
        Assert.Equal(day.Date.AddHours(9).AddMinutes(30), slot.EndsAt);
        Assert.Equal("online", slot.Modality);
    }

    [Fact]
    public async Task CreateAsync_WhenAppointmentConflicts_ShouldFail()
    {
        var day = NextDayOfWeek(DayOfWeek.Monday);
        var appointmentRepository = new InMemoryAppointmentRepository(new Appointment { TenantId = 1, PsychologistId = 10, StartsAt = day.Date.AddHours(9), EndsAt = day.Date.AddHours(10), Status = "scheduled" });
        var service = new AppointmentService(appointmentRepository, NullLogger<AppointmentService>.Instance, AvailabilityFor(day), new InMemoryScheduleBlockRepository());

        var result = await service.CreateAsync(AppointmentDto(day, startsHour: 9, endsHour: 10), CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task CreateAsync_WhenTwoAppointmentsUseSameSlot_ShouldAllowOnlyFirst()
    {
        var day = NextDayOfWeek(DayOfWeek.Monday);
        var appointmentRepository = new InMemoryAppointmentRepository();
        var service = new AppointmentService(appointmentRepository, NullLogger<AppointmentService>.Instance, AvailabilityFor(day), new InMemoryScheduleBlockRepository());

        var first = await service.CreateAsync(AppointmentDto(day, startsHour: 9, endsHour: 10), CancellationToken.None);
        var second = await service.CreateAsync(AppointmentDto(day, startsHour: 9, endsHour: 10), CancellationToken.None);

        Assert.True(first.IsSuccess);
        Assert.False(second.IsSuccess);
    }

    [Fact]
    public async Task CreateAsync_WhenSlotIsBlocked_ShouldFail()
    {
        var day = NextDayOfWeek(DayOfWeek.Monday);
        var blockRepository = new InMemoryScheduleBlockRepository(new ScheduleBlock { TenantId = 1, StartsAt = day.Date.AddHours(9), EndsAt = day.Date.AddHours(10) });
        var service = new AppointmentService(new InMemoryAppointmentRepository(), NullLogger<AppointmentService>.Instance, AvailabilityFor(day), blockRepository);

        var result = await service.CreateAsync(AppointmentDto(day, startsHour: 9, endsHour: 10), CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task CancelAppointmentAsync_WhenLessThan24Hours_ShouldMarkLateCancel()
    {
        var appointment = AppointmentWithId(1, startsAt: DateTime.UtcNow.AddHours(2));
        var repository = new InMemoryAppointmentRepository(appointment);
        var service = new AgendaService(new InMemoryAvailabilityRepository(), new InMemoryScheduleBlockRepository(), repository);

        var result = await service.CancelAppointmentAsync(1, new CancelAppointmentRequest("late"), tenantId: 1, userId: 20, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(appointment.LateCancel);
    }

    [Fact]
    public async Task CancelAppointmentAsync_WhenMoreThan24Hours_ShouldNotMarkLateCancel()
    {
        var appointment = AppointmentWithId(1, startsAt: DateTime.UtcNow.AddDays(2));
        var repository = new InMemoryAppointmentRepository(appointment);
        var service = new AgendaService(new InMemoryAvailabilityRepository(), new InMemoryScheduleBlockRepository(), repository);

        var result = await service.CancelAppointmentAsync(1, new CancelAppointmentRequest("normal"), tenantId: 1, userId: 20, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(appointment.LateCancel);
    }

    [Fact]
    public async Task GetAgendaAsync_ShouldReturnOnlyTenantAppointments()
    {
        var day = NextDayOfWeek(DayOfWeek.Monday);
        var repository = new InMemoryAppointmentRepository(
            new Appointment { TenantId = 1, PatientId = 1, PsychologistId = 10, StartsAt = day.Date.AddHours(9), EndsAt = day.Date.AddHours(10), Status = "scheduled" },
            new Appointment { TenantId = 2, PatientId = 2, PsychologistId = 20, StartsAt = day.Date.AddHours(9), EndsAt = day.Date.AddHours(10), Status = "scheduled" });
        var service = new AgendaService(new InMemoryAvailabilityRepository(), new InMemoryScheduleBlockRepository(), repository);

        var result = await service.GetAgendaAsync(day.Date, day.Date.AddDays(1), tenantId: 1, patientId: null, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
        Assert.Equal(1, result.Value!.Single().PatientId);
    }

    private static DateTime NextDayOfWeek(DayOfWeek dayOfWeek)
    {
        var today = DateTime.UtcNow.Date;
        var daysUntil = ((int)dayOfWeek - (int)today.DayOfWeek + 7) % 7;
        return today.AddDays(daysUntil == 0 ? 7 : daysUntil);
    }

    private static InMemoryAvailabilityRepository AvailabilityFor(DateTime day) => new(new Availability
    {
        TenantId = 1,
        Weekday = (int)day.DayOfWeek,
        StartTime = new TimeOnly(9, 0),
        EndTime = new TimeOnly(17, 0),
        SlotDurationMinutes = 60,
        Modality = "online",
        IsActive = true
    });

    private static CreateAppointmentDTO AppointmentDto(DateTime day, int startsHour, int endsHour) => new(1, "Consulta", 2, 10, day.Date.AddHours(startsHour), day.Date.AddHours(endsHour), "online", "scheduled", 20);

    private static Appointment AppointmentWithId(int id, DateTime startsAt)
    {
        var appointment = new Appointment { TenantId = 1, PatientId = 2, PsychologistId = 10, StartsAt = startsAt, EndsAt = startsAt.AddHours(1), Status = "scheduled" };
        typeof(Core.Domain.Aggregates.BaseEntity<int>)
            .GetProperty(nameof(Core.Domain.Aggregates.BaseEntity<int>.Id), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)!
            .SetValue(appointment, id);
        return appointment;
    }
}

internal sealed class InMemoryAvailabilityRepository(params Availability[] availabilities) : IAvailabilityRepository
{
    private readonly List<Availability> _items = availabilities.ToList();

    public Task<Availability?> GetById(int id, CancellationToken cancellationToken = default) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
    public Task<IEnumerable<Availability?>> GetAll(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Availability?>>(_items);
    public Task<IEnumerable<Availability?>> Find(Expression<Func<Availability, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Availability?>>(_items.AsQueryable().Where(predicate).ToList());
    public Task Create(Availability entity, CancellationToken cancellationToken = default) { _items.Add(entity); return Task.CompletedTask; }
    public Task CreateRange(IEnumerable<Availability> entities, CancellationToken cancellationToken = default) { _items.AddRange(entities); return Task.CompletedTask; }
    public Task Update(Availability entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Delete(Availability entity, CancellationToken cancellationToken = default) { _items.Remove(entity); return Task.CompletedTask; }
    public Task<bool> GetOverlappingAsync(int tenantId, int weekday, string modality, TimeOnly startTime, TimeOnly endTime, int? excludedId, CancellationToken cancellationToken = default) => Task.FromResult(_items.Any(x => x.TenantId == tenantId && x.Id != excludedId && x.Weekday == weekday && x.Modality == modality && x.StartTime < endTime && x.EndTime > startTime));
    public Task ReplaceTenantWeekAsync(int tenantId, IEnumerable<Availability> availabilities, CancellationToken cancellationToken = default) { _items.RemoveAll(x => x.TenantId == tenantId); _items.AddRange(availabilities); return Task.CompletedTask; }
}

internal sealed class InMemoryScheduleBlockRepository(params ScheduleBlock[] blocks) : IScheduleBlockRepository
{
    private readonly List<ScheduleBlock> _items = blocks.ToList();

    public Task<ScheduleBlock?> GetById(int id, CancellationToken cancellationToken = default) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
    public Task<IEnumerable<ScheduleBlock?>> GetAll(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<ScheduleBlock?>>(_items);
    public Task<IEnumerable<ScheduleBlock?>> Find(Expression<Func<ScheduleBlock, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<ScheduleBlock?>>(_items.AsQueryable().Where(predicate).ToList());
    public Task Create(ScheduleBlock entity, CancellationToken cancellationToken = default) { _items.Add(entity); return Task.CompletedTask; }
    public Task CreateRange(IEnumerable<ScheduleBlock> entities, CancellationToken cancellationToken = default) { _items.AddRange(entities); return Task.CompletedTask; }
    public Task Update(ScheduleBlock entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Delete(ScheduleBlock entity, CancellationToken cancellationToken = default) { _items.Remove(entity); return Task.CompletedTask; }
    public Task<bool> ExistsForPeriodAsync(int tenantId, DateTime startsAt, DateTime endsAt, CancellationToken cancellationToken = default) => Task.FromResult(_items.Any(x => x.TenantId == tenantId && x.StartsAt < endsAt && x.EndsAt > startsAt));
    public Task<IReadOnlyCollection<ScheduleBlock>> ListForPeriodAsync(int tenantId, DateTime from, DateTime to, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<ScheduleBlock>>(_items.Where(x => x.TenantId == tenantId && x.StartsAt < to && x.EndsAt > from).ToList());
}

internal sealed class InMemoryAppointmentRepository(params Appointment[] appointments) : IAppointmentRepository
{
    private readonly List<Appointment> _items = appointments.ToList();

    public Task<Appointment?> GetById(int id, CancellationToken cancellationToken = default) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
    public Task<IEnumerable<Appointment?>> GetAll(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Appointment?>>(_items);
    public Task<IEnumerable<Appointment?>> Find(Expression<Func<Appointment, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Appointment?>>(_items.AsQueryable().Where(predicate).ToList());
    public Task Create(Appointment entity, CancellationToken cancellationToken = default) { _items.Add(entity); return Task.CompletedTask; }
    public Task CreateRange(IEnumerable<Appointment> entities, CancellationToken cancellationToken = default) { _items.AddRange(entities); return Task.CompletedTask; }
    public Task Update(Appointment entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Delete(Appointment entity, CancellationToken cancellationToken = default) { _items.Remove(entity); return Task.CompletedTask; }
    public Task<(IEnumerable<Appointment> Items, int TotalCount)> FindByFilter(AppointmentQueryFilter filter, CancellationToken cancellationToken = default) => Task.FromResult(((IEnumerable<Appointment>)_items, _items.Count));
    public Task<IReadOnlyCollection<Appointment>> ListForPeriodAsync(int tenantId, DateTime from, DateTime to, string excludeStatus, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<Appointment>>(_items.Where(x => x.TenantId == tenantId && x.Status != excludeStatus && x.StartsAt < to && x.EndsAt > from).ToList());
    public Task<Appointment?> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken = default) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id && x.TenantId == tenantId));
    public Task<bool> CreateIfSlotIsFreeAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        var hasConflict = _items.Any(x => x.TenantId == appointment.TenantId && x.PsychologistId == appointment.PsychologistId && x.Status != "canceled" && x.StartsAt < appointment.EndsAt && x.EndsAt > appointment.StartsAt);
        if (hasConflict) return Task.FromResult(false);
        _items.Add(appointment);
        return Task.FromResult(true);
    }
}
