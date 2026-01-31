namespace ContactManager.Utilities
{
    // Non-generic: use when success/fail + message needed
    public class OperationResult
    {
        public bool Success { get; init; }
        public string? Message { get; init; }

        public static OperationResult Ok() =>
            new OperationResult { Success = true };

        public static OperationResult Fail(string message) =>
            new OperationResult { Success = false, Message = message };
    }

    // Generic: use when returning data (typed)
    public class OperationResult<T> : OperationResult
    {
        public T? Data { get; init; }

        public static OperationResult<T> Ok(T data) =>
            new OperationResult<T> { Success = true, Data = data };

        public static new OperationResult<T> Fail(string message) =>
            new OperationResult<T> { Success = false, Message = message };
    }
}
