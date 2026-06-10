using Core.Domain.Filters;
using Core.Infrastructure.Repositories;
using Patients.Domain.Aggregates;
using Patients.Domain.Filters;
using Patients.Domain.Filters.Specifications;
using Patients.Domain.Repositories;
using PsiFlow.Patients.Infrastructure.Persistence;

namespace Patients.Infrastructure.Persistence.Repositories;

public sealed class PatientRepository(PatientsDbContext dbContext) : Repository<Patient, int, PatientQueryFilter>(dbContext), IPatientRepository
{
    protected override Specification<Patient, int> CreateSpecification(PatientQueryFilter filter) => new PatientSpecification(filter);
}
