using BuildingBlocks.Results;

namespace OnlineSession.Domain.Errors
{
    public static class VideoRoomErrors
    {
        public static Error InvalidId => Error.Failure("Id invalido.");
        public static Error CreateError => Error.Failure("Erro ao criar VideoRoom.");
        public static Error UpdateError => Error.Failure("Erro ao atualizar VideoRoom.");
        public static Error DeleteError => Error.Failure("Erro ao excluir VideoRoom.");
        public static Error NotFound(int id) => Error.NotFound("VideoRoom nao encontrado: " + id);
        public static Error InvalidProvider => Error.Failure("Provedor invalido. Use zoom, google_meet ou external.");
        public static Error UrlNotHttps => Error.Failure("URL deve ser HTTPS.");
        public static Error UrlSchemeNotAllowed => Error.Failure("Esquema de URL nao permitido.");
        public static Error InstructionsTooLong(int max) => Error.Failure($"Instrucoes devem ter no maximo {max} caracteres.");
        public static Error VideoLinkNotConfigured => Error.NotFound("Link de video ainda nao foi configurado.");
    }
}
