namespace OnlineJobs.Application.Iterators
{
    public interface IIterator<T>
    {
        bool HasNext();
        T Next();
        void Reset();
    }
}
