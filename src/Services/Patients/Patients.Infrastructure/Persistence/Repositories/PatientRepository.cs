using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using Patients.Domain.Aggregates;
using Patients.Domain.Filters;
using Patients.Domain.Repositories;
using PsiFlow.Patients.Infrastructure.Persistence;

namespace Patients.Infrastructure.Persistence.Repositories;

public sealed class PatientRepository(PatientsDbContext dbContext) : Repository<Patient, int, PatientQueryFilter>(dbContext), IPatientRepository
{
    protected override Specification<Patient, int> CreateSpecification(PatientQueryFilter filter) => new PatientSpecification(filter);

    private sealed class PatientSpecification : Specification<Patient, int>
    {
        public PatientSpecification(PatientQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.TenantId.HasValue, x => x.TenantId == filter.TenantId!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.Search), x => x.Name.Contains(filter.Search!) || x.FullName.Contains(filter.Search!) || x.Email.Contains(filter.Search!));
        }
    }
}
