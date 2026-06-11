using Agenda.Application.DTOs.Appointment;
using Core.Application.Interfaces;

namespace Agenda.Application.Contracts
{
    public interface IAppointmentService :
        IReadService<AppointmentDTO, int, AppointmentFilterDTO>,
        ICreateService<CreateAppointmentDTO>,
        IUpdateService<UpdateAppointmentDTO>,
        IDeleteService<int>
    {
    }
}
