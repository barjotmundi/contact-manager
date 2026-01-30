namespace ContactManager.Utilities
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }

        public static OperationResult Ok(object? data = null) => new OperationResult { Success = true, Data = data };
        public static OperationResult Fail(string message) => new OperationResult { Success = false, Message = message };
    }
}
