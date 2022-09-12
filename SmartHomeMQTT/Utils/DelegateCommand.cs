using System;
using System.Windows.Input;

namespace SmartHomeMQTT.Utils
{
    /// <summary>
    /// Typed command to relay Execute &amp; CanExecute calls.
    /// </summary>
    /// <typeparam name="T">The command parameter type</typeparam>
    public class DelegateCommand<T> : ICommand
    {
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Constructor without CanExecute condition.
        /// </summary>
        /// <param name="execute">The action to invoke</param>
        public DelegateCommand(Action<T> execute) : this(execute, null) { }

        /// <summary>
        /// Constructor with CanExecute condition.
        /// </summary>
        /// <param name="execute">The action to invoke</param>
        /// <param name="canExecute">The condition to fulfill</param>
        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// ICommand implementation for CanExecute.
        /// </summary>
        /// <param name="parameter">The command parameter</param>
        /// <returns>Whether the condition of the provided method is fulfilled</returns>
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);

        /// <summary>
        /// ICommand implementation for Execute.
        /// </summary>
        /// <param name="parameter">The command parameter</param>
        public void Execute(object parameter) => _execute((T)parameter);

        /// <summary>
        /// Manually forces a reevaluation of CanExecute.
        /// </summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Typeless command to relay Execute &amp; CanExecute calls
    /// without parameters.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Constructor without CanExecute condition.
        /// </summary>
        /// <param name="execute">The action to invoke</param>
        public DelegateCommand(Action execute) : this(execute, () => true) { }

        /// <summary>
        /// Constructor with CanExecute condition.
        /// </summary>
        /// <param name="execute">The action to invoke</param>
        /// <param name="canExecute">The condition to fulfill</param>
        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// ICommand implementation for CanExecute.
        /// </summary>
        /// <param name="parameter">The command parameter. Unused here.</param>
        /// <returns>Whether the condition of the provided method is fulfilled</returns>
        public bool CanExecute(object parameter) => _canExecute();

        /// <summary>
        /// ICommand implementation for Execute.
        /// </summary>
        /// <param name="parameter">The command parameter. Unused here.</param>
        public void Execute(object parameter) => _execute();

        /// <summary>
        /// Manually forces a reevaluation of CanExecute.
        /// </summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
