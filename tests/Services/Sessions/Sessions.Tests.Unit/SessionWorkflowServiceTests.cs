using Microsoft.Extensions.Logging.Abstractions;
using Sessions.Application.Contracts;
using Sessions.Application.DTOs.Session;
using Sessions.Application.Services;
using Sessions.Domain.Entities;
using Sessions.Domain.Filters;
using Sessions.Domain.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace PsiFlow.Sessions.Tests.Unit;

public sealed class SessionWorkflowServiceTests
{
    [Fact]
    public async Task ChangeStatusAsync_ShouldMoveScheduledToInProgressAndRecordHistory()
    {
        var session = TestSession(1, startsAt: DateTime.UtcNow.AddMinutes(-1));
        var history = new InMemorySessionStatusHistoryRepository();
        var service = Workflow(new InMemorySessionRepository(session), history);

        var result = await service.ChangeStatusAsync(1, new ChangeSessionStatusRequest("in_progress", "started"), 1, 20, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("in_progress", session.Status);
        Assert.Single(history.Items);
    }

    [Fact]
    public async Task ChangeStatusAsync_ShouldRejectScheduledToCompleted()
    {
        var service = Workflow(new InMemorySessionRepository(TestSession(1, startsAt: DateTime.UtcNow.AddMinutes(-1))));

        var result = await service.ChangeStatusAsync(1, new ChangeSessionStatusRequest("completed", null), 1, 20, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task CreateFromAppointmentAsync_ShouldBeIdempotent()
    {
        var repository = new InMemorySessionRepository();
        var service = new SessionService(repository, NullLogger<SessionService>.Instance);
        var dto = new CreateSessionDTO(1, "Consulta", 100, 2, 3, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1));

        var first = await service.CreateFromAppointmentAsync(dto, CancellationToken.None);
        var second = await service.CreateFromAppointmentAsync(dto, CancellationToken.None);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.Single(repository.Items);
        Assert.Equal(first.Value!.Id, second.Value!.Id);
    }

    [Fact]
    public async Task MarkPaymentReceivedAsync_ShouldPersistPaymentData()
    {
        var service = Workflow(new InMemorySessionRepository(TestSession(1, startsAt: DateTime.UtcNow.AddDays(1))));

        var result = await service.MarkPaymentReceivedAsync(1, new MarkPaymentReceivedRequest(15000, "BRL", "ok"), 1, 20, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("received", result.Value!.Status);
        Assert.Equal(15000, result.Value.AmountCents);
        Assert.Equal(20, result.Value.MarkedBy);
    }

    [Fact]
    public async Task GetPatientSessionsAsync_ShouldBeTenantAware()
    {
        var repository = new InMemorySessionRepository(
            TestSession(1, startsAt: DateTime.UtcNow.AddDays(1), tenantId: 1, patientId: 2),
            TestSession(2, startsAt: DateTime.UtcNow.AddDays(1), tenantId: 2, patientId: 2));
        var service = Workflow(repository);

        var result = await service.GetPatientSessionsAsync(2, 1, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
        Assert.Equal(1, result.Value!.Single().Id);
    }

    private static SessionWorkflowService Workflow(InMemorySessionRepository sessionRepository, InMemorySessionStatusHistoryRepository? historyRepository = null) =>
        new(sessionRepository, historyRepository ?? new InMemorySessionStatusHistoryRepository(), new InMemoryManualPaymentRepository(), new InMemoryReceiptRepository());

    private static Session TestSession(int id, DateTime startsAt, int tenantId = 1, int patientId = 2)
    {
        var session = new Session { TenantId = tenantId, Name = "Consulta", AppointmentId = id + 100, PatientId = patientId, PsychologistId = 3, StartsAt = startsAt, EndsAt = startsAt.AddHours(1), Status = "scheduled", Modality = "online" };
        SetId(session, id);
        return session;
    }

    private static void SetId<T>(T entity, int id) where T : Core.Domain.Aggregates.BaseEntity<int> =>
        typeof(Core.Domain.Aggregates.BaseEntity<int>).GetProperty(nameof(Core.Domain.Aggregates.BaseEntity<int>.Id), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)!.SetValue(entity, id);
}

internal sealed class InMemorySessionRepository(params Session[] sessions) : ISessionRepository
{
    public List<Session> Items { get; } = sessions.ToList();
    private int _nextId = sessions.Length + 1;
    public Task<Session?> GetById(int id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
    public Task<IEnumerable<Session?>> GetAll(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Session?>>(Items);
    public Task<IEnumerable<Session?>> Find(Expression<Func<Session, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Session?>>(Items.AsQueryable().Where(predicate).ToList());
    public Task Create(Session entity, CancellationToken cancellationToken = default) { if (entity.Id == 0) typeof(Core.Domain.Aggregates.BaseEntity<int>).GetProperty(nameof(Core.Domain.Aggregates.BaseEntity<int>.Id), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)!.SetValue(entity, _nextId++); Items.Add(entity); return Task.CompletedTask; }
    public Task CreateRange(IEnumerable<Session> entities, CancellationToken cancellationToken = default) { Items.AddRange(entities); return Task.CompletedTask; }
    public Task Update(Session entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Delete(Session entity, CancellationToken cancellationToken = default) { Items.Remove(entity); return Task.CompletedTask; }
    public Task<(IEnumerable<Session> Items, int TotalCount)> FindByFilter(SessionQueryFilter filter, CancellationToken cancellationToken = default) => Task.FromResult(((IEnumerable<Session>)Items, Items.Count));
    public Task<Session?> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id && x.TenantId == tenantId));
    public Task<Session?> GetByAppointmentAndTenantAsync(int appointmentId, int tenantId, CancellationToken cancellationToken) => Task.FromResult(Items.FirstOrDefault(x => x.AppointmentId == appointmentId && x.TenantId == tenantId));
    public Task<IReadOnlyCollection<Session>> ListByPatientOrderedAsync(int patientId, int tenantId, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<Session>>(Items.Where(x => x.PatientId == patientId && x.TenantId == tenantId).OrderBy(x => x.StartsAt).ToList());
}

internal sealed class InMemorySessionStatusHistoryRepository : ISessionStatusHistoryRepository
{
    public List<SessionStatusHistory> Items { get; } = new();
    public Task<SessionStatusHistory?> GetById(int id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
    public Task<IEnumerable<SessionStatusHistory?>> GetAll(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<SessionStatusHistory?>>(Items);
    public Task<IEnumerable<SessionStatusHistory?>> Find(Expression<Func<SessionStatusHistory, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<SessionStatusHistory?>>(Items.AsQueryable().Where(predicate).ToList());
    public Task Create(SessionStatusHistory entity, CancellationToken cancellationToken = default) { Items.Add(entity); return Task.CompletedTask; }
    public Task CreateRange(IEnumerable<SessionStatusHistory> entities, CancellationToken cancellationToken = default) { Items.AddRange(entities); return Task.CompletedTask; }
    public Task Update(SessionStatusHistory entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Delete(SessionStatusHistory entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

internal sealed class InMemoryManualPaymentRepository : IManualPaymentRepository
{
    private readonly List<ManualPayment> _items = new();
    public Task<ManualPayment?> GetById(int id, CancellationToken cancellationToken = default) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
    public Task<IEnumerable<ManualPayment?>> GetAll(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<ManualPayment?>>(_items);
    public Task<IEnumerable<ManualPayment?>> Find(Expression<Func<ManualPayment, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<ManualPayment?>>(_items.AsQueryable().Where(predicate).ToList());
    public Task Create(ManualPayment entity, CancellationToken cancellationToken = default) { _items.Add(entity); return Task.CompletedTask; }
    public Task CreateRange(IEnumerable<ManualPayment> entities, CancellationToken cancellationToken = default) { _items.AddRange(entities); return Task.CompletedTask; }
    public Task Update(ManualPayment entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Delete(ManualPayment entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task<ManualPayment?> GetBySessionAndTenantAsync(int sessionId, int tenantId, CancellationToken cancellationToken) => Task.FromResult(_items.FirstOrDefault(x => x.SessionId == sessionId && x.TenantId == tenantId));
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

internal sealed class InMemoryReceiptRepository : IReceiptRepository
{
    public Task<Receipt?> GetById(int id, CancellationToken cancellationToken = default) => Task.FromResult<Receipt?>(null);
    public Task<IEnumerable<Receipt?>> GetAll(CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Receipt?>>(Array.Empty<Receipt>());
    public Task<IEnumerable<Receipt?>> Find(Expression<Func<Receipt, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IEnumerable<Receipt?>>(Array.Empty<Receipt>());
    public Task Create(Receipt entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task CreateRange(IEnumerable<Receipt> entities, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Update(Receipt entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Delete(Receipt entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task<Receipt?> GetBySessionPaymentAsync(int sessionId, int paymentId, int tenantId, CancellationToken cancellationToken) => Task.FromResult<Receipt?>(null);
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
