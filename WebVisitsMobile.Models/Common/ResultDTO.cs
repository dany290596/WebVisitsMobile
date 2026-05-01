namespace WebVisitsMobile.Models.Common
{
    public class ResultDTO<T>
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
        public T? Value { get; init; }

        public static ResultDTO<T> Ok(T value) => new ResultDTO<T> { Success = true, Value = value };
        public static ResultDTO<T> Fail(string message) => new ResultDTO<T> { Success = false, ErrorMessage = message };
    }
}