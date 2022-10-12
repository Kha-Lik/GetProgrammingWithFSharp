using System;
using System.Windows.Input;

namespace Capstone5;

public class Command<T> : ICommand
{
    private readonly Func<bool> _canExecute;
    private readonly Action<T> _command;
    private readonly Func<object, Tuple<bool, T>> _tryParse;

    public Command(Action<T> command, Func<object, Tuple<bool, T>> tryParse, Func<bool> canExecute = null)
    {
        _command = command;
        _tryParse = tryParse;
        _canExecute = canExecute ?? (() => true);
    }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
        return _canExecute();
    }

    public void Execute(object parameter)
    {
        var result = _tryParse(parameter);
        if (!result.Item1) return;
        _command(result.Item2);
        Refresh();
    }

    public void Refresh()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}