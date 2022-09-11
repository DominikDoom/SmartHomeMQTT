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

        public ObservableCollection<WindowSensor> WindowSensors { get => Get(); set => Set(value); }
        public ObservableCollection<ThermoSensor> ThermoSensors { get => Get(); set => Set(value); }
        public ObservableCollection<OutletSensor> OutletSensors { get => Get(); set => Set(value); }

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
            WindowSensors = new();
            ThermoSensors = new();
            OutletSensors = new();
        }

        private DelegateCommand<string> _publishCommand;
        public ICommand PublishCommand => _publishCommand ??= new(
            (s) => SendMessage(s),
            (s) => CanSendMessage);

        private DelegateCommand _addSensorCommand;
        public ICommand AddSensorCommand => _addSensorCommand ??= new(AddSensor);

        private void AddSensor()
        {
            AddSensorDialog d = new(async (newSensor) =>
            {
                switch (newSensor)
                {
                    case WindowSensor ws:
                        WindowSensors.Add(ws);
                        break;

                    case ThermoSensor ts:
                        ThermoSensors.Add(ts);
                        break;

                    case OutletSensor os:
                        OutletSensors.Add(os);
                        break;
                }
                await ClientHandler.Subscribe(newSensor.Topic);
            });
            _ = d.ShowDialog();
        }

        private static void SendMessage(string message)
            => _ = ClientHandler.Publish("shmqtt/livingroom/window/0", message);
        private bool CanSendMessage => PublishMessage?.Trim().Length > 0;

        private DelegateCommand<Guid> _toggleCommand;
        public ICommand ToggleCommand => _toggleCommand ??= new((id) => ToggleSensor(id));
        private void ToggleSensor(Guid id)
        {
            List<WindowSensor> wSensors = new(WindowSensors);
            List<ThermoSensor> tSensors = new(ThermoSensors);
            List<OutletSensor> oSensors = new(OutletSensors);

            if (wSensors.Find(s => s.Id == id) is WindowSensor ws)
            {
                ws?.Toggle();
                return;
            }
            if (tSensors.Find(s => s.Id == id) is ThermoSensor ts)
            {
                ts?.Toggle();
                return;
            }
            if (oSensors.Find(s => s.Id == id) is OutletSensor os)
            {
                os?.Toggle();
                return;
            }
        }
    }
}
