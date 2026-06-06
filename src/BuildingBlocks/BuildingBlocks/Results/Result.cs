namespace BuildingBlocks.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public Error? Error { get; }

        protected Result(bool isSuccess, Error? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success()
        {
            var result = new Result(true, null);
            return result;
        }

        public static Result Failure(Error error)
        {
            var result = new Result(false, error);
            return result;
        }

        public static Result<T> Success<T>(T value)
        {
            var result = Result<T>.Success(value);
            return result;
        }

        public static Result<T> Failure<T>(Error error)
        {
            var result = Result<T>.Failure(error);
            return result;
        }
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        protected Result(bool isSuccess, T? value, Error? error) : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value)
        {
            var result = new Result<T>(true, value, null);
            return result;
        }

        public new static Result<T> Failure(Error error)
        {
            var result = new Result<T>(false, default, error);
            return result;
        }
    }
}