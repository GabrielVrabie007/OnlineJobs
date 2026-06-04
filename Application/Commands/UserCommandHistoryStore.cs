using System.Collections.Concurrent;

namespace OnlineJobs.Application.Commands
{
    /// <summary>
    /// Holds one <see cref="CommandHistory"/> per user so that Undo/Redo survives
    /// across HTTP requests. A request-scoped history would start empty on every
    /// request, making undo impossible — which is exactly the bug this fixes.
    /// Registered as a singleton; access is keyed by user id and thread-safe.
    /// </summary>
    public class UserCommandHistoryStore
    {
        private readonly ConcurrentDictionary<Guid, CommandHistory> _histories = new();

        /// <summary>Returns the caller's personal command history, creating it on first use.</summary>
        public CommandHistory GetForUser(Guid userId)
            => _histories.GetOrAdd(userId, _ => new CommandHistory());

        public void ClearForUser(Guid userId)
        {
            if (_histories.TryGetValue(userId, out var history))
                history.Clear();
        }
    }
}
