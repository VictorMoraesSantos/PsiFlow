using Core.Application.DTO;
using Core.Application.Interfaces;
using Patients.Application.DTOs.Patient;
using Patients.Domain.Filters;

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
