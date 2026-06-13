using Core.Domain.Repositories;
using Patients.Domain.Entities;

namespace Patients.Domain.Repositories
{
    public interface IPatientStatusHistoryRepository : IRepository<PatientStatusHistory, int>
    {
    }
}
