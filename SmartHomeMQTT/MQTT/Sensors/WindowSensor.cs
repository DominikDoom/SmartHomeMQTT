using MQTTnet.Client;
using SmartHomeMQTT.Utils;
using System;
using System.Text;

namespace SmartHomeMQTT.MQTT.Sensors
{
    public class WindowSensor : GenericSensor
    {
        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                if (value == _isOpen)
                    return;

                _isOpen = value;
                OnPropertyChanged();
                PublishStatus();
            }
        }

        public WindowSensor(Guid id, string room, string name) : base(id, room, "window", name) { }

        public override void PublishStatus() =>
            _ = ClientHandler.Publish(Topic, IsOpen.ToString());

        public override void HandleMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            if (e.ApplicationMessage.Topic != TopicString.TOPIC_COMM)
                return;

            string[] messageParts = Encoding.UTF8.GetString(e.ApplicationMessage.Payload).Split(",");

            if (messageParts[2] == Id.ToString() && messageParts[3] == "toggle")
                Toggle();
        }

        private void Toggle() => IsOpen = !IsOpen;
    }
}
