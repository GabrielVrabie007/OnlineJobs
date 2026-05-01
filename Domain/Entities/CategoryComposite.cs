namespace OnlineJobs.Domain.Entities;

public class CategoryComposite : JobCategory
{
    public List<JobCategory> Children { get; set; }

    public CategoryComposite(string name, string description) : base(name, description)
    {
        Children = new List<JobCategory>();
    }

    protected CategoryComposite() : base(string.Empty, string.Empty)
    {
        Children = new List<JobCategory>();
    }
    
    public override void Add(JobCategory category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        if (!Children.Contains(category))
        {
            Children.Add(category);
        }
    }


    public override void Remove(JobCategory category)
    {
        Children.Remove(category);
    }


    public override JobCategory GetChild(int index)
    {
        if (index < 0 || index >= Children.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        return Children[index];
    }

    public override int GetJobCount()
    {
        int total = 0;
        foreach (var child in Children ?? new List<JobCategory>())
        {
            total += child.GetJobCount();
        }
        return total;
    }


    public override void Display(int depth = 0)
    {
        string indent = new string('-', depth * 2);
        Console.WriteLine($"{indent} {Name} ({GetJobCount()} total jobs)");

        foreach (var child in Children ?? new List<JobCategory>())
        {
            child.Display(depth + 1); // Recursive display
        }
    }


    public override List<CategoryLeaf> GetAllLeafCategories()
    {
        var leafs = new List<CategoryLeaf>();

        foreach (var child in Children ?? new List<JobCategory>())
        {
            leafs.AddRange(child.GetAllLeafCategories());
        }

        return leafs;
    }

    public override bool IsComposite() => true;


    public int GetChildCount() => Children?.Count ?? 0;
}