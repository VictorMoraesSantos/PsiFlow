using BuildingBlocks.Results;
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
        Task<Result<PatientDTO?>> GetByIdAndTenantAsync(int id, int tenantId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<PatientDTO?>>> ListByTenantAsync(int tenantId, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteAsync(int id, int tenantId, CancellationToken cancellationToken = default);
        Task<Result<PatientAdministrativeProfileDTO>> GetAdministrativeProfileAsync(int id, int tenantId, CancellationToken cancellationToken = default);
        Task<Result<PatientDTO>> PatchAdministrativeProfileAsync(int id, int tenantId, PatchPatientAdministrativeDTO dto, CancellationToken cancellationToken = default);
        Task<Result<PatientAdministrativeNoteDTO>> AddAdministrativeNoteAsync(int id, int tenantId, int userId, CreatePatientAdministrativeNoteDTO dto, CancellationToken cancellationToken = default);
        Task<Result<PatientDTO>> UpsertEmergencyContactAsync(int patientId, int tenantId, UpsertEmergencyContactDTO dto, CancellationToken cancellationToken = default);
        Task<Result<ResponsibleLegalDTO>> UpsertResponsibleLegalAsync(int patientId, int tenantId, UpsertResponsibleLegalDTO dto, CancellationToken cancellationToken = default);
        Task<Result<ResponsibleLegalDTO?>> GetResponsibleLegalAsync(int patientId, int tenantId, CancellationToken cancellationToken = default);
    }
}
