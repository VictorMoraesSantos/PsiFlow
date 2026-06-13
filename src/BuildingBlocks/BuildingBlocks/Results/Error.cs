namespace BuildingBlocks.Results
{
    public record Error
    {
        public static readonly Error None = new(string.Empty, ErrorType.Failure);
        public static readonly Error NullValue = new("Null value was provided", ErrorType.Failure);

        public string Description { get; }
        public ErrorType Type { get; }

        public Error(string description, ErrorType type)
        {
            Description = description;
            Type = type;
        }

        public static Error Failure(string description)
        {
            var error = new Error(description, ErrorType.Failure);
            return error;
        }

        public static Error NotFound(string description)
        {
            var error = new Error(description, ErrorType.NotFound);
            return error;
        }

        public static Error Problem(string description)
        {
            var error = new Error(description, ErrorType.Problem);
            return error;
        }

        public static Error Conflict(string description)
        {
            var error = new Error(description, ErrorType.Conflict);
            return error;
        }

        public static Error Forbidden(string description)
        {
            var error = new Error(description, ErrorType.Forbidden);
            return error;
        }
    }
}
