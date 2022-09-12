using SmartHomeMQTT.UI.ViewModels;
using System;
using System.Windows;

namespace SmartHomeMQTT.UI
{
    /// <summary>
    /// Interaction logic for ChangeTempDialog.xaml
    /// </summary>
    public partial class ChangeTempDialog : Window
    {
        /// <summary>
        /// Callback reference to pass back the new temperature to the caller of this dialog.
        /// </summary>
        private Action<int> OnSuccess { get; }

        /// <summary>
        /// Creates a new ChangeTempDialog with the initial temperature set to the
        /// provided value.
        /// </summary>
        /// <param name="currentTemp">The temperature to start with</param>
        /// <param name="onSuccess">The callback to pass back the new temperature on success</param>
        public ChangeTempDialog(int currentTemp, Action<int> onSuccess)
        {
            InitializeComponent();
            // Subscribe to Closing for cleanup
            Closing += HandleClosing;

            // Subscribe to the ViewModel's save and cancel events
            ChangeTempViewModel.SaveEvent += HandleSaveEvent;
            ChangeTempViewModel.CancelEvent += HandleCancelEvent;

            // Assign callback to invoke it outside of the constructor
            OnSuccess = onSuccess;
            // Set textbox to start with previous/current temperature setting
            (DataContext as ChangeTempViewModel).TempValue = currentTemp;
        }

        /// <summary>
        /// Cleans up the <see cref="ChangeTempViewModel.SaveEvent"/> and <see cref="ChangeTempViewModel.CancelEvent"/>
        /// references.
        /// </summary>
        private void HandleClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ChangeTempViewModel.SaveEvent -= HandleSaveEvent;
            ChangeTempViewModel.CancelEvent -= HandleCancelEvent;
        }

        /// <summary>
        /// Closes the window after the cancel event was invoked.
        /// </summary>
        private void HandleCancelEvent(object sender, EventArgs e) => SystemCommands.CloseWindow(this);

        /// <summary>
        /// Invokes the callback and passes back the new temperature
        /// </summary>
        /// <param name="sender">The event sender. Usually null here</param>
        /// <param name="e">The new temperature to pass back</param>
        private void HandleSaveEvent(object sender, int i)
        {
            DialogResult = true;
            OnSuccess.Invoke(i);
            SystemCommands.CloseWindow(this);
        }
    }
}
