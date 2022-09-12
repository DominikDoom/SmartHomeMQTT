using SmartHomeMQTT.Utils;
using System;
using System.Windows.Input;

namespace SmartHomeMQTT.UI.ViewModels
{
    /// <summary>
    /// ViewModel for ChangeTempDialog.xaml.
    /// </summary>
    public class ChangeTempViewModel : Bindable
    {
        /// <summary>
        /// Event called when the save button is clicked.
        /// Expects the saved temperature as an argument.
        /// </summary>
        public static event EventHandler<int> SaveEvent;
        /// <summary>
        /// Event called when the cancel button is clicked.
        /// </summary>
        public static event EventHandler CancelEvent;

        /// <summary>
        /// The temperature value.
        /// <br/>
        /// Bound to a TextBox.
        /// </summary>
        public int? TempValue
        {
            get => Get();
            set
            {
                Set(value);
                _saveCommand?.RaiseCanExecuteChanged();
            }
        }

        private DelegateCommand _saveCommand;
        /// <summary>
        /// The command for saving.
        /// </summary>
        public ICommand SaveCommand => _saveCommand ??= new(Save);

        private DelegateCommand _cancelCommand;
        /// <summary>
        /// The command for cancelling
        /// </summary>
        public ICommand CancelCommand => _cancelCommand ??= new(Cancel);

        /// <summary>
        /// Invokes <see cref="SaveEvent"/>
        /// </summary>
        private void Save() => SaveEvent?.Invoke(null, TempValue.Value);

        /// <summary>
        /// Invokes <see cref="CancelEvent"/>
        /// </summary>
        private static void Cancel() => CancelEvent?.Invoke(null, null);
    }
}
