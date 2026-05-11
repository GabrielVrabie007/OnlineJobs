namespace OnlineJobs.Application.Commands
{
    public interface ICommand
    {
        Task ExecuteAsync();
        Task UndoAsync();
        string GetDescription();
    }
}
