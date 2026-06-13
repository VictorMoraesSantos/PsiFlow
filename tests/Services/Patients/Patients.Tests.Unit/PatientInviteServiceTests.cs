using BuildingBlocks.Results;
using Patients.Application.Services;
using Patients.Domain.Entities;
using Patients.Domain.Events;
using Patients.Domain.Filters;
using Patients.Domain.Repositories;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace PsiFlow.Patients.Tests.Unit;

public sealed class PatientInviteServiceTests
{
    [Fact]
    public async Task CreateInviteAsync_WhenPendingInviteExists_ShouldFail()
    {
        var inviteRepository = new InMemoryPatientInviteRepository();
        inviteRepository.Invites.Add(new PatientInvite
        {
            TenantId = 1,
            Email = "maria@example.com",
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        });

        var service = CreateService(inviteRepository: inviteRepository);

        var result = await service.CreateInviteAsync("maria@example.com", null, null, 1, 10, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task AcceptInviteAsync_WhenInviteIsExpired_ShouldFail()
    {
        var token = "expired-token";
        var inviteRepository = new InMemoryPatientInviteRepository();
        inviteRepository.Invites.Add(new PatientInvite
        {
            TenantId = 1,
            Email = "maria@example.com",
            TokenHash = PatientInviteServiceHash.Hash(token),
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddMinutes(-1)
        });

        var service = CreateService(inviteRepository: inviteRepository);

        var result = await service.AcceptInviteAsync(token, 99, "maria@example.com", "127.0.0.1", "test-agent", CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task AcceptInviteAsync_WhenValid_ShouldRecordAcceptanceAndLinkUser()
    {
        var token = "valid-token";
        var patient = PatientWithId(7, tenantId: 1);
        var patientRepository = new InMemoryPatientRepository(patient);
        var invite = PatientInviteWithId(3, token, patient.Id, tenantId: 1);
        var inviteRepository = new InMemoryPatientInviteRepository(invite);
        var service = CreateService(patientRepository, inviteRepository);

        var result = await service.AcceptInviteAsync(token, 99, "maria@example.com", "127.0.0.1", "test-agent", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("accepted", invite.Status);
        Assert.Equal(99, invite.AcceptedByUserId);
        Assert.Equal("127.0.0.1", invite.AcceptedByIp);
        Assert.Equal("test-agent", invite.AcceptedByUserAgent);
        Assert.Equal(99, patient.UserId);
        Assert.Contains(invite.DomainEvents, e => e is PatientInviteAcceptedDomainEvent);
    }

    [Fact]
    public async Task AcceptInviteAsync_WhenAuthenticatedEmailDoesNotMatch_ShouldFail()
    {
        var token = "valid-token";
        var invite = PatientInviteWithId(3, token, patientId: 7, tenantId: 1);
        var inviteRepository = new InMemoryPatientInviteRepository(invite);
        var service = CreateService(inviteRepository: inviteRepository);

        var result = await service.AcceptInviteAsync(token, 99, "other@example.com", "127.0.0.1", "test-agent", CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Forbidden, result.Error!.Type);
        Assert.Equal("pending", invite.Status);
    }

    [Fact]
    public async Task DeactivateAsync_WhenTenantMatches_ShouldSoftDeactivateAndPreserveHistory()
    {
        var patient = PatientWithId(7, tenantId: 1);
        var patientRepository = new InMemoryPatientRepository(patient);
        var historyRepository = new InMemoryPatientStatusHistoryRepository();
        var service = CreateService(patientRepository, statusHistoryRepository: historyRepository);

        var result = await service.DeactivateAsync(patient.Id, "requested", tenantId: 1, userId: 42, correlationId: "corr-1", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("inactive", patient.Status);
        Assert.NotNull(patient.DeactivatedAt);
        Assert.Equal("requested", patient.DeactivationReason);
        Assert.False(patientRepository.DeleteCalled);
        Assert.Single(historyRepository.Items);
        Assert.Contains(patient.DomainEvents, e => e is PatientDeactivatedDomainEvent);
    }

    [Fact]
    public async Task DeactivateAsync_WhenTenantDoesNotMatch_ShouldNotUpdatePatient()
    {
        var patient = PatientWithId(7, tenantId: 1);
        var patientRepository = new InMemoryPatientRepository(patient);
        var historyRepository = new InMemoryPatientStatusHistoryRepository();
        var service = CreateService(patientRepository, statusHistoryRepository: historyRepository);

        var result = await service.DeactivateAsync(patient.Id, "requested", tenantId: 2, userId: 42, correlationId: "corr-1", CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Forbidden, result.Error!.Type);
        Assert.Equal("active", patient.Status);
        Assert.False(patientRepository.UpdateCalled);
        Assert.Empty(historyRepository.Items);
    }

    private static PatientInviteService CreateService(
        InMemoryPatientRepository? patientRepository = null,
        InMemoryPatientInviteRepository? inviteRepository = null,
        InMemoryPatientStatusHistoryRepository? statusHistoryRepository = null) =>
        new(
            patientRepository ?? new InMemoryPatientRepository(),
            statusHistoryRepository ?? new InMemoryPatientStatusHistoryRepository(),
            inviteRepository ?? new InMemoryPatientInviteRepository());

    private static Patient PatientWithId(int id, int tenantId)
    {
        var patient = new Patient { TenantId = tenantId, FullName = "Maria Silva", Email = "maria@example.com", Phone = "11999999999" };
        SetId(patient, id);
        return patient;
    }

    private static PatientInvite PatientInviteWithId(int id, string token, int patientId, int tenantId)
    {
        var invite = new PatientInvite
        {
            TenantId = tenantId,
            PatientId = patientId,
            Email = "maria@example.com",
            TokenHash = PatientInviteServiceHash.Hash(token),
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        SetId(invite, id);
        return invite;
    }

    private static void SetId<T>(T entity, int id) where T : Core.Domain.Aggregates.BaseEntity<int> =>
        typeof(Core.Domain.Aggregates.BaseEntity<int>)
            .GetProperty(nameof(Core.Domain.Aggregates.BaseEntity<int>.Id), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(entity, id);
}

internal static class PatientInviteServiceHash
{
    public static string Hash(string value) => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value)));
}

internal sealed class InMemoryPatientRepository(params Patient[] patients) : IPatientRepository
{
    private readonly List<Patient> _patients = patients.ToList();
    public bool DeleteCalled { get; private set; }
    public bool UpdateCalled { get; private set; }

    public Task<Patient?> GetById(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_patients.FirstOrDefault(x => x.Id == id));

    public Task<IEnumerable<Patient?>> GetAll(CancellationToken cancellationToken = default) =>
        Task.FromResult<IEnumerable<Patient?>>(_patients);

    public Task<IEnumerable<Patient?>> Find(Expression<Func<Patient, bool>> predicate, CancellationToken cancellationToken = default) =>
        Task.FromResult<IEnumerable<Patient?>>(_patients.AsQueryable().Where(predicate).ToList());

    public Task Create(Patient entity, CancellationToken cancellationToken = default)
    {
        _patients.Add(entity);
        return Task.CompletedTask;
    }

    public Task CreateRange(IEnumerable<Patient> entities, CancellationToken cancellationToken = default)
    {
        _patients.AddRange(entities);
        return Task.CompletedTask;
    }

    public Task Update(Patient entity, CancellationToken cancellationToken = default)
    {
        UpdateCalled = true;
        return Task.CompletedTask;
    }

    public Task Delete(Patient entity, CancellationToken cancellationToken = default)
    {
        DeleteCalled = true;
        _patients.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<(IEnumerable<Patient> Items, int TotalCount)> FindByFilter(PatientQueryFilter filter, CancellationToken cancellationToken = default) =>
        Task.FromResult(((IEnumerable<Patient>)_patients, _patients.Count));

    public Task<Patient?> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_patients.FirstOrDefault(x => x.Id == id && x.TenantId == tenantId));
}

internal sealed class InMemoryPatientInviteRepository(params PatientInvite[] invites) : IPatientInviteRepository
{
    public List<PatientInvite> Invites { get; } = invites.ToList();

    public Task<PatientInvite?> GetById(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Invites.FirstOrDefault(x => x.Id == id));

    public Task<IEnumerable<PatientInvite?>> GetAll(CancellationToken cancellationToken = default) =>
        Task.FromResult<IEnumerable<PatientInvite?>>(Invites);

    public Task<IEnumerable<PatientInvite?>> Find(Expression<Func<PatientInvite, bool>> predicate, CancellationToken cancellationToken = default) =>
        Task.FromResult<IEnumerable<PatientInvite?>>(Invites.AsQueryable().Where(predicate).ToList());

    public Task Create(PatientInvite entity, CancellationToken cancellationToken = default)
    {
        Invites.Add(entity);
        return Task.CompletedTask;
    }

    public Task CreateRange(IEnumerable<PatientInvite> entities, CancellationToken cancellationToken = default)
    {
        Invites.AddRange(entities);
        return Task.CompletedTask;
    }

    public Task Update(PatientInvite entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Delete(PatientInvite entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<bool> HasPendingForEmailAsync(int tenantId, string email, CancellationToken cancellationToken = default) =>
        Task.FromResult(Invites.Any(x => x.TenantId == tenantId && x.Email == email && x.Status == "pending" && x.ExpiresAt > DateTime.UtcNow));

    public Task<PatientInvite?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        Task.FromResult(Invites.FirstOrDefault(x => x.TokenHash == tokenHash));

    public Task<PatientInvite?> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Invites.FirstOrDefault(x => x.Id == id && x.TenantId == tenantId));
}

internal sealed class InMemoryPatientStatusHistoryRepository : IPatientStatusHistoryRepository
{
    public List<PatientStatusHistory> Items { get; } = new();

    public Task<PatientStatusHistory?> GetById(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Items.FirstOrDefault(x => x.Id == id));

    public Task<IEnumerable<PatientStatusHistory?>> GetAll(CancellationToken cancellationToken = default) =>
        Task.FromResult<IEnumerable<PatientStatusHistory?>>(Items);

    public Task<IEnumerable<PatientStatusHistory?>> Find(Expression<Func<PatientStatusHistory, bool>> predicate, CancellationToken cancellationToken = default) =>
        Task.FromResult<IEnumerable<PatientStatusHistory?>>(Items.AsQueryable().Where(predicate).ToList());

    public Task Create(PatientStatusHistory entity, CancellationToken cancellationToken = default)
    {
        Items.Add(entity);
        return Task.CompletedTask;
    }

    public Task CreateRange(IEnumerable<PatientStatusHistory> entities, CancellationToken cancellationToken = default)
    {
        Items.AddRange(entities);
        return Task.CompletedTask;
    }

    public Task Update(PatientStatusHistory entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Delete(PatientStatusHistory entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
