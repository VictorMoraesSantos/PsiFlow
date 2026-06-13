using Core.Application.Interfaces;
using Sessions.Application.DTOs.Session;

namespace Sessions.Application.Contracts
{
    public interface ISessionService :
        IReadService<SessionDTO, int, SessionFilterDTO>,
        ICreateService<CreateSessionDTO>,
        IUpdateService<UpdateSessionDTO>,
        IDeleteService<int>
    {
        Task<BuildingBlocks.Results.Result<SessionDTO>> CreateFromAppointmentAsync(CreateSessionDTO dto, CancellationToken cancellationToken = default);
        Task<BuildingBlocks.Results.Result<bool>> CancelByAppointmentAsync(int appointmentId, int tenantId, string? reason, CancellationToken cancellationToken = default);
    }
}
