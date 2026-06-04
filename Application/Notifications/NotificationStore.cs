using System.Collections.Concurrent;

namespace OnlineJobs.Application.Notifications
{
    /// <summary>
    /// Per-user inbox of in-app notifications. This is what the user actually sees
    /// in the navbar bell and the Notifications page — the real, visible output of
    /// the Observer and Decorator patterns. Registered as a singleton; thread-safe.
    /// </summary>
    public class NotificationStore
    {
        private const int MaxPerUser = 50;
        private readonly ConcurrentDictionary<Guid, List<UserNotification>> _byUser = new();

        public void Add(Guid userId, string title, string message, string icon = "bi-bell")
        {
            if (userId == Guid.Empty) return;
            var list = _byUser.GetOrAdd(userId, _ => new List<UserNotification>());
            lock (list)
            {
                list.Insert(0, new UserNotification(userId, title, message, icon));
                if (list.Count > MaxPerUser)
                    list.RemoveRange(MaxPerUser, list.Count - MaxPerUser);
            }
        }

        public IReadOnlyList<UserNotification> GetForUser(Guid userId)
        {
            if (!_byUser.TryGetValue(userId, out var list)) return Array.Empty<UserNotification>();
            lock (list) { return list.ToList(); }
        }

        public int UnreadCount(Guid userId)
        {
            if (!_byUser.TryGetValue(userId, out var list)) return 0;
            lock (list) { return list.Count(n => !n.IsRead); }
        }

        public void MarkAllRead(Guid userId)
        {
            if (!_byUser.TryGetValue(userId, out var list)) return;
            lock (list) { foreach (var n in list) n.IsRead = true; }
        }
    }
}
