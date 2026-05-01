using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.DTOs;

public class ApplicationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public JobApplication? Application { get; set; }
    public List<string> ValidationErrors { get; set; } = new();

    public static ApplicationResult Successful(JobApplication application)
    {
        return new ApplicationResult
        {
            Success = true,
            Message = "Application submitted successfully!",
            Application = application
        };
    }

    public static ApplicationResult Failed(string message, List<string>? errors = null)
    {
        return new ApplicationResult
        {
            Success = false,
            Message = message,
            ValidationErrors = errors ?? new List<string>()
        };
    }
}