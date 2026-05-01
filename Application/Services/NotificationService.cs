using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Notifications;
using OnlineJobs.Application.Decorators;

namespace OnlineJobs.Application.Services;


public class NotificationService : INotificationService
{

    private INotification CreateFullNotification()
    {
        INotification notification = new BaseNotification();
        notification = new EmailNotificationDecorator(notification);
        notification = new SMSNotificationDecorator(notification);
        notification = new PushNotificationDecorator(notification);
        notification = new LoggingNotificationDecorator(notification);

        return notification;
    }


    private INotification CreateEmailNotification()
    {
        INotification notification = new BaseNotification();
        notification = new EmailNotificationDecorator(notification);
        notification = new LoggingNotificationDecorator(notification);

        return notification;
    }

    public async Task SendApplicationConfirmationAsync(Guid jobSeekerId, string jobTitle)
    {
        var notification = CreateFullNotification();

        await notification.SendAsync(
            recipient: $"jobseeker-{jobSeekerId}@example.com",
            subject: "Application Submitted Successfully",
            message: $"Your application for '{jobTitle}' has been submitted successfully. The employer will review it soon."
        );
    }

    public async Task NotifyEmployerNewApplicationAsync(Guid employerId, string jobSeekerName, string jobTitle)
    {
        INotification notification = new BaseNotification();
        notification = new EmailNotificationDecorator(notification);
        notification = new PushNotificationDecorator(notification);
        notification = new LoggingNotificationDecorator(notification);

        await notification.SendAsync(
            recipient: $"employer-{employerId}@company.com",
            subject: "New Job Application Received",
            message: $"{jobSeekerName} has applied for your job posting '{jobTitle}'. Review the application in your dashboard."
        );
    }

    public async Task SendProfileCompletionReminderAsync(Guid jobSeekerId)
    {
        var notification = CreateEmailNotification();

        await notification.SendAsync(
            recipient: $"jobseeker-{jobSeekerId}@example.com",
            subject: "Complete Your Profile",
            message: "Your profile is incomplete. Please add your education, work experience, and skills to improve your chances of getting hired."
        );
    }
}