using SmartHomeMQTT.MQTT;
using SmartHomeMQTT.MQTT.Sensors;
using SmartHomeMQTT.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartHomeMQTT.UI.ViewModels
{
    public class DashboardViewModel : Bindable
    {
        public ObservableCollection<string> TopicMessages { get => Get(); set => Set(value); }

        public ObservableCollection<WindowSensor> Sensors { get => Get(); set => Set(value); }

        public string PublishMessage
        {
            get => Get();
            set
            {
                Set(value);
                _publishCommand.RaiseCanExecuteChanged();
            }
        }

        public DashboardViewModel()
        {
            TopicMessages = new();
            Sensors = new();
        }

        private DelegateCommand<string> _publishCommand;
        public ICommand PublishCommand => _publishCommand ??= new(
            (s) => SendMessage(s),
            (s) => CanSendMessage);

        private static void SendMessage(string message)
            => _ = ClientHandler.Publish("shmqtt/livingroom/window/0", message);

        private bool CanSendMessage => PublishMessage?.Trim().Length > 0;

        private DelegateCommand<Guid> _toggleSensorCommand;
        public ICommand ToggleSensorCommand => _toggleSensorCommand ??= new((id) => ToggleSensor(id));

        private void ToggleSensor(Guid id)
        {
            List<WindowSensor> sensors = new(Sensors);
            sensors.Find(s => s.Id == id).Toggle();
        }
    }
}
