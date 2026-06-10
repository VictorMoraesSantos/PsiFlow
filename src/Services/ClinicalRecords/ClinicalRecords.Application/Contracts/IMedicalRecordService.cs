using ClinicalRecords.Application.DTOs.MedicalRecord;
using Core.Application.DTO;
using Core.Application.Interfaces;

namespace ClinicalRecords.Application.Contracts
{
    public interface IMedicalRecordService :
        IReadService<MedicalRecordDTO, int, MedicalRecordFilterDTO>,
        ICreateService<CreateMedicalRecordDTO>,
        IUpdateService<UpdateMedicalRecordDTO>,
        IDeleteService<int>
    {
    }
}
