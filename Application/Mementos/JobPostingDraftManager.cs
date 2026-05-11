namespace OnlineJobs.Application.Mementos
{
    public class JobPostingDraftManager
    {
        private readonly Dictionary<Guid, Stack<JobPostingMemento>> _drafts = new();
        private const int MaxDraftsPerJob = 5;

        public void SaveDraft(Guid jobPostingId, JobPostingMemento memento)
        {
            if (!_drafts.ContainsKey(jobPostingId))
            {
                _drafts[jobPostingId] = new Stack<JobPostingMemento>();
            }

            var stack = _drafts[jobPostingId];
            stack.Push(memento);

            if (stack.Count > MaxDraftsPerJob)
            {
                var tempStack = new Stack<JobPostingMemento>(stack.Reverse().Skip(1));
                _drafts[jobPostingId] = new Stack<JobPostingMemento>(tempStack.Reverse());
            }
        }

        public JobPostingMemento? GetLatestDraft(Guid jobPostingId)
        {
            if (_drafts.ContainsKey(jobPostingId) && _drafts[jobPostingId].Count > 0)
            {
                return _drafts[jobPostingId].Peek();
            }
            return null;
        }

        public JobPostingMemento? RestoreDraft(Guid jobPostingId)
        {
            if (_drafts.ContainsKey(jobPostingId) && _drafts[jobPostingId].Count > 0)
            {
                return _drafts[jobPostingId].Pop();
            }
            return null;
        }

        public List<JobPostingMemento> GetAllDrafts(Guid jobPostingId)
        {
            if (_drafts.ContainsKey(jobPostingId))
            {
                return _drafts[jobPostingId].ToList();
            }
            return new List<JobPostingMemento>();
        }

        public void DeleteAllDrafts(Guid jobPostingId)
        {
            if (_drafts.ContainsKey(jobPostingId))
            {
                _drafts[jobPostingId].Clear();
            }
        }
    }
}
