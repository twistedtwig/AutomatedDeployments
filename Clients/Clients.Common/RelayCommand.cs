using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Clients.Common
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public string Header { get; set; }
        public string ToolTip { get; set; }
        public string IconName { get; set; }

        public event EventHandler Executed;

        public RelayCommand(string header, Action<object> execute) : this(header, execute, null) { }
        public RelayCommand(string header, Action<object> execute, Predicate<object> canExecute) : this(execute, canExecute) 
        { 
            Header = header;
        }

        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
            OnExecuted();
        }

        protected virtual void OnExecuted()
        {
            var handler = Executed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
