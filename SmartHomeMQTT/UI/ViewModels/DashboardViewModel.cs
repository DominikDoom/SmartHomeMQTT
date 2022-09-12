using SmartHomeMQTT.MQTT;
using SmartHomeMQTT.MQTT.Sensors;
using SmartHomeMQTT.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartHomeMQTT.UI.ViewModels
{
    /// <summary>
    /// ViewModel for Dashboard.xaml
    /// </summary>
    public class DashboardViewModel : Bindable
    {
        /// <summary>
        /// The client handler used for subscribing to the created sensors.
        /// </summary>
        public ClientHandler ClientHandler { get; }

        /// <summary>
        /// A collection of all topic messages received on subscribed channels during the application lifetime.
        /// Dynamically updates.
        /// </summary>
        public ObservableCollection<string> TopicMessages { get => Get(); set => Set(value); }

        /// <summary>
        /// Collection of all window sensors for data binding
        /// </summary>
        public ObservableCollection<WindowSensor> WindowSensors { get => Get(); set => Set(value); }
        /// <summary>
        /// Collection of all thermostat sensors for data binding
        /// </summary>
        public ObservableCollection<ThermoSensor> ThermoSensors { get => Get(); set => Set(value); }
        /// <summary>
        /// Collection of all outlet sensors for data binding
        /// </summary>
        public ObservableCollection<OutletSensor> OutletSensors { get => Get(); set => Set(value); }

        /// <summary>
        /// Creates necessary ObservableCollection instances
        /// as well as the ClientHandler.
        /// </summary>
        public DashboardViewModel()
        {
            TopicMessages = new();
            WindowSensors = new();
            ThermoSensors = new();
            OutletSensors = new();

            ClientHandler = new();
        }

        private DelegateCommand _addSensorCommand;
        /// <summary>
        /// Command for adding a new sensor.
        /// </summary>
        public ICommand AddSensorCommand => _addSensorCommand ??= new(AddSensor);

        /// <summary>
        /// Opens the <see cref="AddSensorDialog"/> in a new window and uses
        /// the return value of the passed Action to add the new sensor.
        /// </summary>
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

                    default:
                        throw new NotImplementedException();
                }
                await ClientHandler.Subscribe(newSensor.Topic);
            });
            _ = d.ShowDialog();
        }

        private DelegateCommand<Guid> _toggleCommand;
        /// <summary>
        /// The command to toggle a sensor.
        /// The respective sensor's id is passed from UI as the command parameter.
        /// </summary>
        public ICommand ToggleCommand => _toggleCommand ??= new((id) => ToggleSensor(id));

        /// <summary>
        /// Publishes a new message on the global channel informing subscribed sensors of the toggle request.
        /// </summary>
        /// <param name="id">
        /// The id of the sensor to toggle. 
        /// Other sensors listen to this event too to automatically react to specific conditions.
        /// </param>
        private async void ToggleSensor(Guid id)
        {
            List<WindowSensor> wSensors = new(WindowSensors);
            List<ThermoSensor> tSensors = new(ThermoSensors);
            List<OutletSensor> oSensors = new(OutletSensors);

            string room = "";
            string type = "";
            bool toggleState = false;
            // Find the sensor by id and use pattern matching to assign it as the correct type
            if (wSensors.Find(s => s.Id == id) is WindowSensor ws)
            {
                room = ws.Room;
                type = "window";
                toggleState = !ws.IsOpen;
            }
            else if (tSensors.Find(s => s.Id == id) is ThermoSensor ts)
            {
                room = ts.Room;
                type = "thermo";
                toggleState = !ts.IsOn;
            }
            else if (oSensors.Find(s => s.Id == id) is OutletSensor os)
            {
                room = os.Room;
                type = "outlet";
                toggleState = !os.IsOn;
            }

            // Publish the toggle request message
            // Contains (in order, comma-separated): Room, sensor type, sensor id, "toggle", True/False depending on the toggle state
            await ClientHandler.Publish($"{TopicString.TOPIC_COMM}", $"{room},{type},{id},toggle,{toggleState}");
        }

        private DelegateCommand<Guid> _setTempCommand;
        /// <summary>
        /// Command to set the temparature of a thermostat.
        /// The respective sensor's id is passed from UI as the command parameter.
        /// </summary>
        public ICommand SetTempCommand => _setTempCommand ??= new((id) => SetTemp(id));

        /// <summary>
        /// Opens a dialog where the user can set the new temperature for the respective thermostat.
        /// Publishes a new message on the global channel informing subscribed sensors of the set temperature request.
        /// </summary>
        /// <param name="id">
        /// The id of the sensor to toggle. 
        /// Other sensors listen to this event too to automatically react to specific conditions.
        /// </param>
        private void SetTemp(Guid id)
        {
            List<ThermoSensor> tSensors = new(ThermoSensors);

            // Find the specified ThermoSensor
            if (tSensors.Find(s => s.Id == id) is ThermoSensor ts)
            {
                string room = ts.Room;

                // Open the ChangeTempDialog and use the returned temperature in the publish message
                ChangeTempDialog d = new(ts.HighTemp, async (returnedTemp) =>
                {
                    // Publish the settemp request message
                    // Contains (in order, comma-separated): Room, "thermo", sensor id, "settemp", the new temperature as int
                    await ClientHandler.Publish($"{TopicString.TOPIC_COMM}", $"{room},thermo,{id},settemp,{returnedTemp}");
                });
                _ = d.ShowDialog();
            }
        }
    }
}
