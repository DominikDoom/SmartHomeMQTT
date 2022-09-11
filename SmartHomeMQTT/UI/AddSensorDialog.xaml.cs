using SmartHomeMQTT.MQTT.Sensors;
using SmartHomeMQTT.UI.ViewModels;
using System;
using System.Windows;

namespace SmartHomeMQTT.UI
{
    /// <summary>
    /// Interaktionslogik für AddSensorDialog.xaml
    /// </summary>
    public partial class AddSensorDialog : Window
    {
        private Action<GenericSensor> OnSuccess { get; }

        public AddSensorDialog(Action<GenericSensor> onSuccess)
        {
            InitializeComponent();
            Closing += HandleClosing;
            AddSensorViewModel.SaveEvent += HandleSaveEvent;
            AddSensorViewModel.CancelEvent += HandleCancelEvent;

            OnSuccess = onSuccess;
        }

        private void HandleClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AddSensorViewModel.SaveEvent -= HandleSaveEvent;
            AddSensorViewModel.CancelEvent -= HandleCancelEvent;
        }

        private void HandleCancelEvent(object sender, EventArgs e) => SystemCommands.CloseWindow(this);

        private void HandleSaveEvent(object sender, GenericSensor e)
        {
            DialogResult = true;
            OnSuccess.Invoke(e);
            SystemCommands.CloseWindow(this);
        }
    }
}
