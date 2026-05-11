namespace OnlineJobs.Application.Commands
{
    public class CommandHistory
    {
        private readonly Stack<ICommand> _undoStack = new();
        private readonly Stack<ICommand> _redoStack = new();
        private readonly int _maxHistorySize;

        public CommandHistory(int maxHistorySize = 50)
        {
            _maxHistorySize = maxHistorySize;
        }

        public void AddCommand(ICommand command)
        {
            _undoStack.Push(command);
            _redoStack.Clear();

            if (_undoStack.Count > _maxHistorySize)
            {
                var tempStack = new Stack<ICommand>(_undoStack.Reverse().Skip(1));
                _undoStack.Clear();
                foreach (var cmd in tempStack.Reverse())
                {
                    _undoStack.Push(cmd);
                }
            }
        }

        public async Task<bool> UndoAsync()
        {
            if (!CanUndo())
                return false;

            var command = _undoStack.Pop();
            await command.UndoAsync();
            _redoStack.Push(command);

            return true;
        }

        public async Task<bool> RedoAsync()
        {
            if (!CanRedo())
                return false;

            var command = _redoStack.Pop();
            await command.ExecuteAsync();
            _undoStack.Push(command);

            return true;
        }

        public bool CanUndo() => _undoStack.Count > 0;
        public bool CanRedo() => _redoStack.Count > 0;

        public List<string> GetHistory()
        {
            return _undoStack.Select(c => c.GetDescription()).Reverse().ToList();
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
