using MQTTnet.Client;
using SmartHomeMQTT.MQTT.Sensors;
using SmartHomeMQTT.UI.ViewModels;
using System;
using System.Text;
using System.Windows.Controls;

namespace SmartHomeMQTT.UI
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        /// <summary>
        /// ViewModel reference for convenience.
        /// </summary>
        private readonly DashboardViewModel VM;

        public Dashboard()
        {
            InitializeComponent();

            // Set up ViewModel reference
            VM = DataContext as DashboardViewModel;

            InitSensors();
        }

        /// <summary>
        /// Creates and subscribes to a few example sensors.
        /// </summary>
        private async void InitSensors()
        {
            await VM.ClientHandler.Connect();

            // Subscribe to test topic
            WindowSensor ws = new(Guid.NewGuid(), "bathroom", "Main window");
            WindowSensor ws2 = new(Guid.NewGuid(), "bathroom", "Side window");

            await VM.ClientHandler.Subscribe(ws.Topic);
            await VM.ClientHandler.Subscribe(ws2.Topic);

            VM.WindowSensors.Add(ws);
            VM.WindowSensors.Add(ws2);
            VM.ClientHandler.MessageReceivedEvent += HandleMessageReceived;
        }

        /// <summary>
        /// Parses the received messages.
        /// </summary>
        /// <remarks>
        /// Since the client currently holds references to all sensors in memory, it is not
        /// used to update the UI, only for the log messages.
        /// <br/>
        /// However, it is a proof of concept since it could be easily adapted to the former
        /// given more time.
        /// </remarks>
        /// <param name="sender">The event sender. Usually null here</param>
        /// <param name="e">The event parameters containing the message</param>
        private void HandleMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            // Split topic into parts to parse sensor room, type, and id
            string[] splitTopic = e.ApplicationMessage.Topic.Split("/");
            string sensorId = $"{splitTopic[3]}/{splitTopic[4]}/{splitTopic[5]}";
            // Decode the payload
            string message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            string status = "";
            // Generate the log string for the ObservableCollection depending on the sensor type
            switch (splitTopic[4])
            {
                case "window":
                    status = bool.Parse(message)
                        ? "Open"
                        : "Closed";
                    break;

                case "outlet":
                    status = bool.Parse(message)
                        ? "On"
                        : "Off";
                    break;

                case "thermo":
                    string[] messageParts = message.Split(",");
                    string onStatus = bool.Parse(messageParts[0])
                        ? "On"
                        : "Off";
                    string tempStatus = bool.Parse(messageParts[1])
                        ? "Low mode"
                        : "High mode";
                    status = $"{onStatus}, {tempStatus}, low={messageParts[2]} °C, high={messageParts[3]} °C";
                    break;
                default:
                    break;
            };

            // Add the log string to the collection.
            // Needs to be done on the Dispatcher to assure it is added from the UI thread.
            Dispatcher.Invoke(() => VM.TopicMessages.Add($"{sensorId} changed to {status}"));
        }
    }
}
