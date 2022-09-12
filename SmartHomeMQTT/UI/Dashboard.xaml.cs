using MQTTnet.Client;
using SmartHomeMQTT.MQTT.Sensors;
using SmartHomeMQTT.UI.ViewModels;
using System;
using System.Text;
using System.Windows.Controls;

namespace SmartHomeMQTT.UI
{
    /// <summary>
    /// Interaktionslogik für Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        private readonly DashboardViewModel VM;

        public Dashboard()
        {
            InitializeComponent();

            // Set up ViewModel reference
            VM = DataContext as DashboardViewModel;

            InitSensors();
        }

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

        private void HandleMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            string[] splitTopic = e.ApplicationMessage.Topic.Split("/");
            string sensorId = $"{splitTopic[3]}/{splitTopic[4]}/{splitTopic[5]}";
            string message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            string status = "";
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

            Dispatcher.Invoke(() => VM.TopicMessages.Add($"{sensorId} changed to {status}"));
        }
    }
}
