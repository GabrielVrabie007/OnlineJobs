namespace OnlineJobs.Application.Mediators
{
    public class MediatorResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public List<string> Errors { get; set; }

        public MediatorResult()
        {
            Data = new Dictionary<string, object>();
            Errors = new List<string>();
            Message = string.Empty;
        }

        public static MediatorResult SuccessResult(string message, Dictionary<string, object>? data = null)
        {
            return new MediatorResult
            {
                Success = true,
                Message = message,
                Data = data ?? new Dictionary<string, object>()
            };
        }

        public static MediatorResult FailureResult(string message, List<string>? errors = null)
        {
            return new MediatorResult
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
