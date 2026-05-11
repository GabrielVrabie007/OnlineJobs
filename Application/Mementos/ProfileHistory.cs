namespace OnlineJobs.Application.Mementos
{
    public class ProfileHistory
    {
        private readonly Dictionary<Guid, Stack<ProfileMemento>> _history = new();
        private const int MaxHistorySize = 10;

        public void SaveMemento(Guid jobSeekerId, ProfileMemento memento)
        {
            if (!_history.ContainsKey(jobSeekerId))
            {
                _history[jobSeekerId] = new Stack<ProfileMemento>();
            }

            var stack = _history[jobSeekerId];
            stack.Push(memento);

            if (stack.Count > MaxHistorySize)
            {
                var tempStack = new Stack<ProfileMemento>(stack.Reverse().Skip(1));
                _history[jobSeekerId] = new Stack<ProfileMemento>(tempStack.Reverse());
            }
        }

        public ProfileMemento? GetLatestMemento(Guid jobSeekerId)
        {
            if (_history.ContainsKey(jobSeekerId) && _history[jobSeekerId].Count > 0)
            {
                return _history[jobSeekerId].Peek();
            }
            return null;
        }

        public ProfileMemento? RestoreMemento(Guid jobSeekerId)
        {
            if (_history.ContainsKey(jobSeekerId) && _history[jobSeekerId].Count > 0)
            {
                return _history[jobSeekerId].Pop();
            }
            return null;
        }

        public List<ProfileMemento> GetHistory(Guid jobSeekerId)
        {
            if (_history.ContainsKey(jobSeekerId))
            {
                return _history[jobSeekerId].ToList();
            }
            return new List<ProfileMemento>();
        }

        public void ClearHistory(Guid jobSeekerId)
        {
            if (_history.ContainsKey(jobSeekerId))
            {
                _history[jobSeekerId].Clear();
            }
        }
    }
}
