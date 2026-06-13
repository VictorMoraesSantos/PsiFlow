using Core.Domain.Repositories;
using Patients.Domain.Entities;

namespace Patients.Domain.Repositories;

public interface IPatientAdministrativeNoteRepository : IRepository<PatientAdministrativeNote, int>
{
}
