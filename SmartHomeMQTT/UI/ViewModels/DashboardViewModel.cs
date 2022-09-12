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
        public ClientHandler ClientHandler { get; }

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

            ClientHandler = new();
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

        private void SendMessage(string message)
            => _ = ClientHandler.Publish("shmqtt/livingroom/window/0", message);
        private bool CanSendMessage => PublishMessage?.Trim().Length > 0;

        private DelegateCommand<Guid> _toggleCommand;
        public ICommand ToggleCommand => _toggleCommand ??= new((id) => ToggleSensor(id));
        private async void ToggleSensor(Guid id)
        {
            List<WindowSensor> wSensors = new(WindowSensors);
            List<ThermoSensor> tSensors = new(ThermoSensors);
            List<OutletSensor> oSensors = new(OutletSensors);

            string room = "";
            string type = "";
            bool toggleStatus = false;
            if (wSensors.Find(s => s.Id == id) is WindowSensor ws)
            {
                room = ws.Room;
                type = "window";
                toggleStatus = !ws.IsOpen;
            }
            else if (tSensors.Find(s => s.Id == id) is ThermoSensor ts)
            {
                room = ts.Room;
                type = "thermo";
                toggleStatus = !ts.IsOn;
            }
            else if (oSensors.Find(s => s.Id == id) is OutletSensor os)
            {
                room = os.Room;
                type = "outlet";
                toggleStatus = !os.IsOn;
            }

            await ClientHandler.Publish($"{TopicString.TOPIC_COMM}", $"{room},{type},{id},toggle,{toggleStatus}");
        }

        private DelegateCommand<Guid> _setTempCommand;
        public ICommand SetTempCommand => _setTempCommand ??= new((id) => SetTemp(id));
        private void SetTemp(Guid id)
        {
            List<ThermoSensor> tSensors = new(ThermoSensors);

            if (tSensors.Find(s => s.Id == id) is ThermoSensor ts)
            {
                string room = ts.Room;

                ChangeTempDialog d = new(ts.HighTemp, async (returnedTemp) =>
                {
                    await ClientHandler.Publish($"{TopicString.TOPIC_COMM}", $"{room},thermo,{id},settemp,{returnedTemp}");
                });
                _ = d.ShowDialog();
            }
        }
    }
}
