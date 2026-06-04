namespace OnlineJobs.Application.Notifications
{
    /// <summary>An in-app notification shown to a specific user in their bell menu.</summary>
    public class UserNotification
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Guid UserId { get; }
        public string Title { get; }
        public string Message { get; }
        public string Icon { get; }
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public bool IsRead { get; set; }

        public UserNotification(Guid userId, string title, string message, string icon = "bi-bell")
        {
            UserId = userId;
            Title = title;
            Message = message;
            Icon = icon;
        }
    }
}
