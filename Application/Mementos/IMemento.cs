namespace OnlineJobs.Application.Mementos
{
    public interface IMemento
    {
        DateTime CreatedAt { get; }
        string GetDescription();
    }
}
