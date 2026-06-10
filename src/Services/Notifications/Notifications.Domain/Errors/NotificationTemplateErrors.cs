using BuildingBlocks.Results;

namespace Notifications.Domain.Errors
{
    public static class NotificationTemplateErrors
    {
        public static Error InvalidId => Error.Failure("Id invalido.");
        public static Error CreateError => Error.Failure("Erro ao criar NotificationTemplate.");
        public static Error UpdateError => Error.Failure("Erro ao atualizar NotificationTemplate.");
        public static Error DeleteError => Error.Failure("Erro ao excluir NotificationTemplate.");
        public static Error NotFound(int id) => Error.NotFound("NotificationTemplate nao encontrado: " + id);
        public static Error ContainsClinicalData => Error.Failure("Template contem dados clinicos proibidos.");
        public static Error PayloadContainsClinicalData => Error.Failure("Payload contem dados clinicos proibidos.");
        public static Error NoActiveTemplate(string key) => Error.Failure($"Sem template ativo para a chave: {key}");
        public static Error ReminderNotAllowed => Error.Failure("Lembrete nao permitido para sessoes canceladas.");
        public static Error DuplicateReminder => Error.Failure("Ja existe lembrete do mesmo tipo para esta sessao.");
        public static Error InvalidRecipientEmail => Error.Failure("E-mail de destinatario invalido.");
    }
}
