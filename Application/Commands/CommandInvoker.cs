namespace OnlineJobs.Application.Commands
{
    public class CommandInvoker
    {
        private readonly CommandHistory _history;

        public CommandInvoker(CommandHistory history)
        {
            _history = history ?? throw new ArgumentNullException(nameof(history));
        }

        public async Task ExecuteAsync(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            await command.ExecuteAsync();
            _history.AddCommand(command);
        }

        public async Task<bool> UndoAsync()
        {
            return await _history.UndoAsync();
        }

        public async Task<bool> RedoAsync()
        {
            return await _history.RedoAsync();
        }

        public bool CanUndo() => _history.CanUndo();
        public bool CanRedo() => _history.CanRedo();

        public List<string> GetCommandHistory() => _history.GetHistory();
    }
}
