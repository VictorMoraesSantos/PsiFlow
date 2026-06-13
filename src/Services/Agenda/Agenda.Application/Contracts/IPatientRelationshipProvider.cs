using BuildingBlocks.Results;

namespace Agenda.Application.Contracts;

public interface IPatientRelationshipProvider
{
    Task<Result<bool>> IsPatientLinkedToTenantAsync(int patientId, int tenantId, CancellationToken cancellationToken);
}

public sealed class AllowTenantPatientRelationshipProvider : IPatientRelationshipProvider
{
    public Task<Result<bool>> IsPatientLinkedToTenantAsync(int patientId, int tenantId, CancellationToken cancellationToken) =>
        Task.FromResult(Result.Success(patientId > 0 && tenantId > 0));
}
