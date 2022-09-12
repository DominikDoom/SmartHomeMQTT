using SmartHomeMQTT.UI.ViewModels;
using System;
using System.Windows;

namespace SmartHomeMQTT.UI
{
    /// <summary>
    /// Interaktionslogik für ChangeTempDialog.xaml
    /// </summary>
    public partial class ChangeTempDialog : Window
    {
        private Action<int> OnSuccess { get; }

        public ChangeTempDialog(int currentTemp, Action<int> onSuccess)
        {
            InitializeComponent();
            Closing += HandleClosing;
            ChangeTempViewModel.SaveEvent += HandleSaveEvent;
            ChangeTempViewModel.CancelEvent += HandleCancelEvent;

            OnSuccess = onSuccess;
            // Set textbox to start with previous/current temperature setting
            (DataContext as ChangeTempViewModel).TempValue = currentTemp;
        }

        private void HandleClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ChangeTempViewModel.SaveEvent -= HandleSaveEvent;
            ChangeTempViewModel.CancelEvent -= HandleCancelEvent;
        }

        private void HandleCancelEvent(object sender, EventArgs e) => SystemCommands.CloseWindow(this);

        private void HandleSaveEvent(object sender, int i)
        {
            DialogResult = true;
            OnSuccess.Invoke(i);
            SystemCommands.CloseWindow(this);
        }
    }
}
