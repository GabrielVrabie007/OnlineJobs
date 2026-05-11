namespace OnlineJobs.Application.Iterators
{
    public interface ICollection<T>
    {
        IIterator<T> CreateIterator();
    }
}
