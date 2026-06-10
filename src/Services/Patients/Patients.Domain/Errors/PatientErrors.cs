using BuildingBlocks.Results;

namespace Patients.Domain.Errors
{
    public static class PatientErrors
    {
        public static Error InvalidId => Error.Failure("Id invalido.");
        public static Error CreateError => Error.Failure("Erro ao criar Patient.");
        public static Error UpdateError => Error.Failure("Erro ao atualizar Patient.");
        public static Error DeleteError => Error.Failure("Erro ao excluir Patient.");
        public static Error DuplicateEmailInTenant => Error.Failure("Ja existe um paciente com este e-mail neste tenant.");
        public static Error FullNameRequired => Error.Failure("Nome completo e obrigatorio.");
        public static Error EmailRequired => Error.Failure("E-mail e obrigatorio.");
        public static Error PhoneRequired => Error.Failure("Telefone e obrigatorio.");
        public static Error BirthDateInFuture => Error.Failure("Data de nascimento nao pode ser futura.");
        public static Error EmergencyContactPhoneRequired => Error.Failure("Telefone de contato de emergencia e obrigatorio quando nome informado.");
        public static Error NotFound(int id) => Error.NotFound("Patient nao encontrado: " + id);
        public static Error InviteNotFound(int id) => Error.NotFound("Convite nao encontrado: " + id);
        public static Error InviteAlreadyAccepted => Error.Failure("Convite ja foi aceito.");
        public static Error InviteExpired => Error.Failure("Convite expirado.");
        public static Error InviteRevoked => Error.Failure("Convite revogado.");
        public static Error PendingInviteExists => Error.Failure("Ja existe um convite pendente para este e-mail.");
    }
}
