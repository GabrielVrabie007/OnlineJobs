namespace OnlineJobs.Models
{
    /// <summary>Same fields as creating a job, plus the id of the posting being edited.</summary>
    public class EditJobViewModel : CreateJobViewModel
    {
        public System.Guid Id { get; set; }
        public string? Status { get; set; }
    }
}
