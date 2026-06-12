using BuildingBlocks.Results;

namespace Auth.Domain.Errors
{
    public static class OutboxMessageErrors
    {
        public static Error InvalidId => Error.Failure("Id invalido.");
        public static Error AggregateIdRequired => Error.Failure("AggregateId e obrigatorio.");
        public static Error AggregateTypeRequired => Error.Failure("AggregateType e obrigatorio.");
        public static Error EventTypeRequired => Error.Failure("EventType e obrigatorio.");
        public static Error PayloadRequired => Error.Failure("Payload da mensagem e obrigatorio.");
        public static Error OccurredAtRequired => Error.Failure("OccurredAt e obrigatorio.");
        public static Error NotFoundMessage => Error.NotFound("Mensagem de outbox nao encontrada.");
        public static Error AlreadyProcessed => Error.Failure("Mensagem de outbox ja foi processada.");
        public static Error RetryLimitExceeded => Error.Failure("Limite de tentativas excedido para a mensagem de outbox.");
        public static Error EnqueueError => Error.Failure("Falha ao enfileirar mensagem no outbox.");
        public static Error ProcessError => Error.Failure("Falha ao processar mensagem do outbox.");
        public static Error NotFound(Guid correlationId) => Error.NotFound($"Mensagem de outbox nao encontrada para o correlationId: {correlationId}");
    }
}
