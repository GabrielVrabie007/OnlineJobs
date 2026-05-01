namespace OnlineJobs.Application.Interfaces;


public interface INotificationService
{
    Task SendApplicationConfirmationAsync(Guid jobSeekerId, string jobTitle);


    Task NotifyEmployerNewApplicationAsync(Guid employerId, string jobSeekerName, string jobTitle);

  
    Task SendProfileCompletionReminderAsync(Guid jobSeekerId);
}