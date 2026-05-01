namespace OnlineJobs.Application.Interfaces
{

    public interface INotification
    {
        Task SendAsync(string recipient, string subject, string message);
        string GetDescription();
    }
}
