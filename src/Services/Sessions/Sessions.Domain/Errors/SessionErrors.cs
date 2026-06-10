using BuildingBlocks.Results;

namespace Sessions.Domain.Errors
{
    public static class SessionErrors
    {
        public static Error InvalidId => Error.Failure("Id invalido.");
        public static Error CreateError => Error.Failure("Erro ao criar Session.");
        public static Error UpdateError => Error.Failure("Erro ao atualizar Session.");
        public static Error DeleteError => Error.Failure("Erro ao excluir Session.");
        public static Error NotFound(int id) => Error.NotFound("Session nao encontrada: " + id);
        public static Error InvalidTransition(string from, string to) => Error.Failure($"Transicao invalida: {from} -> {to}");
        public static Error AlreadyTerminal(string status) => Error.Failure($"Sessao em estado terminal: {status}");
        public static Error MustBeInProgressToComplete => Error.Failure("Sessao precisa estar in_progress para ser concluida.");
        public static Error StartOutsideWindow => Error.Failure("Inicio fora da janela permitida.");
        public static Error AmountCentsNegative => Error.Failure("Valor do pagamento deve ser >= 0.");
        public static Error PaymentNotReceived => Error.Failure("Pagamento ainda nao foi recebido.");
    }
}
