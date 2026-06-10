using Patients.Domain.Aggregates;
using Patients.Domain.Filters;
using Core.Domain.Repositories;

namespace Patients.Domain.Repositories
{
    public interface IPatientRepository : IRepository<Patient, int, PatientQueryFilter> { }
}
