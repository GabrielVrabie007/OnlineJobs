namespace OnlineJobs.Domain.Entities;


public abstract class JobCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    protected JobCategory(string name, string description)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
    }

    protected JobCategory()
    {
        Id = Guid.NewGuid();
        Name = string.Empty;
        Description = string.Empty;
    }


    public abstract int GetJobCount();

  
    public abstract void Display(int depth = 0);


    public abstract List<CategoryLeaf> GetAllLeafCategories();


    public virtual void Add(JobCategory category)
    {
        throw new NotSupportedException("Cannot add to a leaf category");
    }


    public virtual void Remove(JobCategory category)
    {
        throw new NotSupportedException("Cannot remove from a leaf category");
    }


    public virtual JobCategory GetChild(int index)
    {
        throw new NotSupportedException("Leaf category has no children");
    }


    public virtual bool IsComposite() => false;
}