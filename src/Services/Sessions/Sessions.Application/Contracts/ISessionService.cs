using Core.Application.DTO;
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
    }
}
