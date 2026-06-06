using System.Net;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Results
{
    public class HttpResult : HttpResult<object>
    {
        public HttpResult()
        {
        }

        public HttpResult(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public HttpResult(object data, HttpStatusCode statusCode) : base(data, statusCode)
        {
        }

        public HttpResult(HttpStatusCode statusCode, params string[] errors) : base(statusCode, errors)
        {
        }

        public static HttpResult Ok(object data)
        {
            return new HttpResult(data, HttpStatusCode.OK);
        }

        public static HttpResult Ok(HttpStatusCode statusCode)
        {
            return new HttpResult(HttpStatusCode.OK);
        }

        public static HttpResult Created(object data)
        {
            return new HttpResult(data, HttpStatusCode.Created);
        }

        public static HttpResult Created(HttpStatusCode statusCode)
        {
            return new HttpResult(HttpStatusCode.Created);
        }

        public static HttpResult Updated(HttpStatusCode statusCode)
        {
            return new HttpResult(HttpStatusCode.NoContent);
        }

        public static HttpResult Deleted(HttpStatusCode statusCode)
        {
            return new HttpResult(HttpStatusCode.NoContent);
        }

        public static HttpResult BadRequest(params string[] errors)
        {
            return new HttpResult(HttpStatusCode.BadRequest, errors);
        }

        public static HttpResult NotFound(params string[] errors)
        {
            return new HttpResult(HttpStatusCode.NotFound, errors);
        }

        public static HttpResult Forbidden(params string[] errors)
        {
            return new HttpResult(HttpStatusCode.Forbidden, errors);
        }

        public static HttpResult InternalError(params string[] errors)
        {
            return new HttpResult(HttpStatusCode.InternalServerError, errors);
        }

        public static HttpResult Unauthorized(params string[] errors)
        {
            return new HttpResult(HttpStatusCode.Unauthorized, errors);
        }
    }

    public class HttpResult<T>
    {
        [JsonPropertyOrder(0)] public bool Success => (int)StatusCode >= 200 && (int)StatusCode < 300 && Errors.Length == 0;

        [JsonPropertyOrder(1)] public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        [JsonPropertyOrder(2)] public string[] Errors { get; set; } = Array.Empty<string>();

        [JsonPropertyOrder(3)] public T Data { get; set; }

        [JsonPropertyOrder(4)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PaginationData? Pagination { get; set; }

        public HttpResult()
        {
            Errors = Array.Empty<string>();
        }

        public HttpResult(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpResult(T data, HttpStatusCode statusCode)
        {
            Data = data;
            Errors = Array.Empty<string>();
            StatusCode = statusCode;
        }

        public HttpResult(T data, PaginationData pagination, HttpStatusCode statusCode)
            : this(data, statusCode)
        {
            Pagination = pagination;
        }

        public HttpResult(HttpStatusCode statusCode, params string[] errors)
        {
            StatusCode = statusCode;
            Errors = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToArray() ?? Array.Empty<string>();
        }

        public void AddError(params string[] errors)
        {
            if (errors == null || errors.Length == 0) return;
            Errors = Errors.Concat(errors.Where(e => !string.IsNullOrWhiteSpace(e))).ToArray();
        }

        public static HttpResult<T> Ok(T data)
        {
            return new HttpResult<T>(data, HttpStatusCode.OK);
        }

        public static HttpResult<T> Created(T data)
        {
            return new HttpResult<T>(data, HttpStatusCode.Created);
        }

        public static HttpResult<T> Updated()
        {
            return new HttpResult<T>(HttpStatusCode.NoContent);
        }

        public static HttpResult<T> Deleted()
        {
            return new HttpResult<T>(HttpStatusCode.NoContent);
        }

        public static HttpResult<T> BadRequest(params string[] errors)
        {
            return new HttpResult<T>(HttpStatusCode.BadRequest, errors);
        }

        public static HttpResult<T> NotFound(params string[] errors)
        {
            return new HttpResult<T>(HttpStatusCode.NotFound, errors);
        }

        public static HttpResult<T> Forbidden(params string[] errors)
        {
            return new HttpResult<T>(HttpStatusCode.Forbidden, errors);
        }

        public static HttpResult<T> InternalError(params string[] errors)
        {
            return new HttpResult<T>(HttpStatusCode.InternalServerError, errors);
        }

        public static HttpResult<T> Unauthorized(params string[] errors)
        {
            return new HttpResult<T>(HttpStatusCode.Unauthorized, errors);
        }

        public static HttpResult<T> FromException(Exception ex)
        {
            var errors = new[]
            {
                ex.Message,
                ex.InnerException?.Message
            }.Where(m => !string.IsNullOrWhiteSpace(m)).ToArray();
            var result = new HttpResult<T>(HttpStatusCode.InternalServerError, errors);
            return result;
        }

        public static HttpResult<T> Ok(T data, PaginationData pagination)
        {
            var result = new HttpResult<T>(data, pagination, HttpStatusCode.OK);
            return result;
        }

        public static HttpResult<T> Ok(T data, int currentPage, int pageSize, int totalItems)
        {
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var result = new HttpResult<T>(
                data,
                new PaginationData(currentPage, pageSize, totalItems, totalPages),
                HttpStatusCode.OK);
            return result;
        }

        public static HttpResult<T> FromResult<TData>(Result<(TData Items, PaginationData Pagination)> result)
            where TData : T
        {
            if (!result.IsSuccess)
                return BadRequest(result.Error!.Description);

            var res = Ok(result.Value.Items, result.Value.Pagination);
            return res;
        }
    }
}