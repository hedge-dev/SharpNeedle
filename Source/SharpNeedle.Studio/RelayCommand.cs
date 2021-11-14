using System;
using System.Windows.Input;

namespace SharpNeedle.Studio
{
    public class RelayCommand : ICommand
    {
        public readonly Action ExecuteFunc;
        public readonly Func<bool> CanExecuteFunc;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action executeFunc, Func<bool> canExecuteFunc = null)
        {
            ExecuteFunc = executeFunc;
            CanExecuteFunc = canExecuteFunc;
        }

        public bool CanExecute(object parameter)
        {
            return ExecuteFunc != null && (CanExecuteFunc == null || CanExecuteFunc());
        }

        public void Execute(object parameter)
        {
            ExecuteFunc();
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        public readonly Action<T> ExecuteFunc;
        public readonly Func<T, bool> CanExecuteFunc;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<T> executeFunc, Func<T, bool> canExecuteFunc = null)
        {
            ExecuteFunc = executeFunc;
            CanExecuteFunc = canExecuteFunc;
        }

        public bool CanExecute(object parameter)
        {
            return ExecuteFunc != null && (CanExecuteFunc == null || CanExecuteFunc((T)parameter));
        }

        public void Execute(object parameter)
        {
            ExecuteFunc((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
