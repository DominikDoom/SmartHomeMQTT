using SmartHomeMQTT.Utils;
using System;
using System.Windows.Input;

namespace SmartHomeMQTT.UI.ViewModels
{
    public class ChangeTempViewModel : Bindable
    {
        public static event EventHandler<int> SaveEvent;
        public static event EventHandler CancelEvent;

        public int? TempValue
        {
            get => Get();
            set
            {
                Set(value);
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        private DelegateCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new(Save);

        private DelegateCommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new(Cancel);

        private void Save() => SaveEvent?.Invoke(null, TempValue.Value);
        //private bool CanSave() => TempValue.Trim().Length > 0;

        private static void Cancel() => CancelEvent?.Invoke(null, null);
    }
}
