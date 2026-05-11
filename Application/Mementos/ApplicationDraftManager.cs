namespace OnlineJobs.Application.Mementos
{
    public class ApplicationDraftManager
    {
        private readonly Dictionary<string, ApplicationFormMemento> _drafts = new();

        public void SaveDraft(Guid jobSeekerId, Guid jobPostingId, ApplicationFormMemento memento)
        {
            var key = GetKey(jobSeekerId, jobPostingId);
            _drafts[key] = memento;
        }

        public ApplicationFormMemento? GetDraft(Guid jobSeekerId, Guid jobPostingId)
        {
            var key = GetKey(jobSeekerId, jobPostingId);
            return _drafts.ContainsKey(key) ? _drafts[key] : null;
        }

        public void DeleteDraft(Guid jobSeekerId, Guid jobPostingId)
        {
            var key = GetKey(jobSeekerId, jobPostingId);
            _drafts.Remove(key);
        }

        public List<ApplicationFormMemento> GetAllDraftsForJobSeeker(Guid jobSeekerId)
        {
            return _drafts
                .Where(kvp => kvp.Key.StartsWith($"{jobSeekerId}_"))
                .Select(kvp => kvp.Value)
                .ToList();
        }

        public void ClearOldDrafts(int daysOld = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
            var keysToRemove = _drafts
                .Where(kvp => kvp.Value.CreatedAt < cutoffDate)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                _drafts.Remove(key);
            }
        }

        private static string GetKey(Guid jobSeekerId, Guid jobPostingId)
        {
            return $"{jobSeekerId}_{jobPostingId}";
        }
    }
}
