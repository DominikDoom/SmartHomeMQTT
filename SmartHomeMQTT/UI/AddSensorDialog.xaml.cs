using SmartHomeMQTT.MQTT.Sensors;
using SmartHomeMQTT.UI.ViewModels;
using System;
using System.Windows;

namespace SmartHomeMQTT.UI
{
    /// <summary>
    /// Interaction logic for AddSensorDialog.xaml
    /// </summary>
    public partial class AddSensorDialog : Window
    {
        /// <summary>
        /// Callback reference to pass back the newly created sensor to the caller of this dialog.
        /// </summary>
        private Action<GenericSensor> OnSuccess { get; }

        /// <summary>
        /// Creates a new AddSensorDialog.
        /// </summary>
        /// <param name="onSuccess">The callback to pass back the new sensor on success</param>
        public AddSensorDialog(Action<GenericSensor> onSuccess)
        {
            InitializeComponent();
            // Subscribe to Closing for cleanup
            Closing += HandleClosing;

            // Subscribe to the ViewModel's save and cancel events
            AddSensorViewModel.SaveEvent += HandleSaveEvent;
            AddSensorViewModel.CancelEvent += HandleCancelEvent;

            // Assign callback to invoke it outside of the constructor
            OnSuccess = onSuccess;
        }

        /// <summary>
        /// Cleans up the <see cref="AddSensorViewModel.SaveEvent"/> and <see cref="AddSensorViewModel.CancelEvent"/>
        /// references.
        /// </summary>
        private void HandleClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AddSensorViewModel.SaveEvent -= HandleSaveEvent;
            AddSensorViewModel.CancelEvent -= HandleCancelEvent;
        }

        /// <summary>
        /// Closes the window after the cancel event was invoked.
        /// </summary>
        private void HandleCancelEvent(object sender, EventArgs e) => SystemCommands.CloseWindow(this);

        /// <summary>
        /// Invokes the callback and passes back the newly created sensor.
        /// </summary>
        /// <param name="sender">The event sender. Usually null here</param>
        /// <param name="e">The new sensor to pass back</param>
        private void HandleSaveEvent(object sender, GenericSensor e)
        {
            DialogResult = true;
            OnSuccess.Invoke(e);
            SystemCommands.CloseWindow(this);
        }
    }
}
