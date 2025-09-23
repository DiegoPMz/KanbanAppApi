namespace KanbanAppApi.Responses
{
    public class ApiResponse<T>
    {
        public bool Succeeded { get; init; }
        public string Message { get; init; } = string.Empty;
        public T? Data { get; init; }
        public IEnumerable<string>? Errors { get; init; }

        public ApiResponse(bool succeeded, string message, T? data, IEnumerable<string>? errors = null)
        {
            Succeeded = succeeded;
            Message = message;
            Data = data;
            Errors = errors;
        }

        public static ApiResponse<T> Success(T data, string message)
        {
            return new ApiResponse<T>(true, message, data, default);
        }

        public static ApiResponse<T> Failure(string message, IEnumerable<string> errors)
        {
            return new ApiResponse<T>(false, message, default, errors);
        }
    }
}
