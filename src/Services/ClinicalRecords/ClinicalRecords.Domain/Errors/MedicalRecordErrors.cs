using BuildingBlocks.Results;

namespace ClinicalRecords.Domain.Errors
{
    public static class MedicalRecordErrors
    {
        public static Error InvalidId => Error.Failure("Id invalido.");
        public static Error CreateError => Error.Failure("Erro ao criar MedicalRecord.");
        public static Error UpdateError => Error.Failure("Erro ao atualizar MedicalRecord.");
        public static Error DeleteError => Error.Failure("Erro ao excluir MedicalRecord.");
        public static Error NotFound(int id) => Error.NotFound("MedicalRecord nao encontrado: " + id);
        public static Error TextTooLong(int max) => Error.Failure($"Texto clinico excede o maximo de {max} caracteres.");
        public static Error NoDraftToPublish => Error.Failure("Nao ha rascunho para publicar.");
        public static Error DecryptionFailed => Error.Failure("Falha ao descriptografar conteudo clinico.");
    }
}
