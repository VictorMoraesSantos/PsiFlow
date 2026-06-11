using Core.Application.Interfaces;
using Patients.Application.DTOs.Patient;

namespace Patients.Application.Contracts
{
    public interface IPatientService :
        IReadService<PatientDTO, int, PatientFilterDTO>,
        ICreateService<CreatePatientDTO>,
        IUpdateService<UpdatePatientDTO>,
        IDeleteService<int>
    {
    }
}
