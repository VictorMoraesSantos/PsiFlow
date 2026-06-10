using BuildingBlocks.Results;

namespace Agenda.Domain.Errors
{
    public static class AppointmentErrors
    {
        public static Error InvalidId => Error.Failure("Id invalido.");
        public static Error CreateError => Error.Failure("Erro ao criar Appointment.");
        public static Error UpdateError => Error.Failure("Erro ao atualizar Appointment.");
        public static Error DeleteError => Error.Failure("Erro ao excluir Appointment.");
        public static Error NotFound(int id) => Error.NotFound("Appointment nao encontrado: " + id);
        public static Error OverlappingAvailability => Error.Failure("Janela de disponibilidade sobrepoe outra existente.");
        public static Error OutsideAvailability => Error.Failure("Horario fora da disponibilidade semanal ativa.");
        public static Error BlockedByScheduleBlock => Error.Failure("Horario bloqueado por bloqueio de agenda.");
        public static Error AlreadyCancelled => Error.Failure("Agendamento ja cancelado.");
        public static Error CannotCancelPast => Error.Failure("Nao e possivel cancelar agendamento no passado.");
    }
}
