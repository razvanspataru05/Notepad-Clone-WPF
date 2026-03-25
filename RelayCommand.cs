using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Tema1
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> action;
        private readonly Func<object, bool> predicate;

        public RelayCommand(Action<object> action)
        {
            this.action = action;
        }
        public RelayCommand(Action<object> action, Func<object, bool> predicate)
        {
            this.action = action;
            this.predicate = predicate;
        }
        public void Execute(object parameter)
        {
            action(parameter);
        }
        public bool CanExecute(object? parameter)
        {
            if (predicate == null) return true;

            return predicate(parameter);
        }
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
