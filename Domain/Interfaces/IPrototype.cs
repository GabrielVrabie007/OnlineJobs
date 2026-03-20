namespace OnlineJobs.Domain.Interfaces
{

    public interface IPrototype<T> where T : class
    {

        T Clone();
        T ShallowCopy();
    }
}
