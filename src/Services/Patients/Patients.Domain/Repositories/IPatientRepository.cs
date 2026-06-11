using Core.Domain.Repositories;
using Patients.Domain.Entities;
using Patients.Domain.Filters;

namespace Patients.Domain.Repositories
{
    public interface IPatientRepository : IRepository<Patient, int, PatientQueryFilter> { }
}
