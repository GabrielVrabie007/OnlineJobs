using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Notifications;
using OnlineJobs.Application.Decorators;

namespace OnlineJobs.Application.Services;

/// <summary>
/// Builds a multi-channel notification using the Decorator pattern. The In-App
/// channel delivers to the recipient's notification bell (what they actually see);
/// Email/SMS/Push are simulated external channels and Logging records the send.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly NotificationStore _store;

    public NotificationService(NotificationStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    // One place to stack the channels. Adding/removing a channel is a one-line change
    // here — the whole point of the Decorator pattern.
    private INotification BuildChannels(Guid recipientId, string icon)
    {
        INotification notification = new BaseNotification();
        notification = new EmailNotificationDecorator(notification);
        notification = new SMSNotificationDecorator(notification);
        notification = new PushNotificationDecorator(notification);
        notification = new InAppNotificationDecorator(notification, _store, recipientId, icon);
        notification = new LoggingNotificationDecorator(notification);
        return notification;
    }

    public async Task SendApplicationConfirmationAsync(Guid jobSeekerId, string jobTitle)
    {
        var notification = BuildChannels(jobSeekerId, "bi-send-check");
        await notification.SendAsync(
            recipient: $"jobseeker-{jobSeekerId}@example.com",
            subject: "Application submitted",
            message: $"Your application for '{jobTitle}' was submitted. The employer will review it soon.");
    }

    public async Task NotifyEmployerNewApplicationAsync(Guid employerId, string jobSeekerName, string jobTitle)
    {
        var notification = BuildChannels(employerId, "bi-person-plus");
        await notification.SendAsync(
            recipient: $"employer-{employerId}@company.com",
            subject: "New application received",
            message: $"{jobSeekerName} applied for '{jobTitle}'. Review it in Received Applications.");
    }

    public async Task SendProfileCompletionReminderAsync(Guid jobSeekerId)
    {
        var notification = BuildChannels(jobSeekerId, "bi-person-lines-fill");
        await notification.SendAsync(
            recipient: $"jobseeker-{jobSeekerId}@example.com",
            subject: "Complete your profile",
            message: "Add your education, experience and skills to improve your match score.");
    }
}
