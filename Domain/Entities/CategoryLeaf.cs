namespace OnlineJobs.Domain.Entities;


public class CategoryLeaf : JobCategory
{
    public List<JobPosting> Jobs { get; set; }

    public CategoryLeaf(string name, string description) : base(name, description)
    {
        Jobs = new List<JobPosting>();
    }

    protected CategoryLeaf() : base(string.Empty, string.Empty)
    {
        Jobs = new List<JobPosting>();
    }


    public void AddJob(JobPosting job)
    {
        if (!Jobs.Contains(job))
        {
            Jobs.Add(job);
        }
    }

    public void RemoveJob(JobPosting job)
    {
        Jobs.Remove(job);
    }

    public override int GetJobCount()
    {
        return Jobs?.Count ?? 0;
    }

 
    public override void Display(int depth = 0)
    {
        string indent = new string('-', depth * 2);
        Console.WriteLine($"{indent} {Name} ({GetJobCount()} jobs)");
    }

   
    public override List<CategoryLeaf> GetAllLeafCategories()
    {
        return new List<CategoryLeaf> { this };
    }
}