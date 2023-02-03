using System;
using System.Windows.Input;

namespace SealWatch.Wpf.Extensions;

/// <summary>
/// Custom ICommand to handle WPF commands.
/// </summary>
public class DelegateCommand : ICommand
{
    public Action CommandAction { get; set; }
    public Action<object> ObjectCommandAction { get; set; }
    public Func<bool> CanExecuteFunc { get; set; }

    public void Execute(object parameter)
    {
        if (CommandAction != null)
            CommandAction();
        else
        {
            if (parameter is null) return;
            ObjectCommandAction(parameter);
        }
    }

    public bool CanExecute(object parameter)
    {
        return CanExecuteFunc == null || CanExecuteFunc();
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}