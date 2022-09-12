using MQTTnet.Client;
using SmartHomeMQTT.Utils;
using System;
using System.Text;

namespace SmartHomeMQTT.MQTT.Sensors
{
    public class OutletSensor : GenericSensor
    {
        private bool _isOn = true;
        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (value == _isOn)
                    return;

                _isOn = value;
                OnPropertyChanged();
                PublishStatus();
            }
        }

        public OutletSensor(Guid id, string room, string name) : base(id, room, "outlet", name) { }

        public override void PublishStatus() =>
            _ = ClientHandler.Publish(Topic, IsOn.ToString());

        public override void HandleMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            if (e.ApplicationMessage.Topic != TopicString.TOPIC_COMM)
                return;

            string[] messageParts = Encoding.UTF8.GetString(e.ApplicationMessage.Payload).Split(",");

            // This sensor should be toggled
            if (messageParts[2] == Id.ToString() && messageParts[3] == "toggle")
                Toggle();

            // A window in the same room was opened, turn on/off if after 22:00
            if (messageParts[0] == Room
                && messageParts[1] == "window"
                && messageParts[3] == "toggle")
            {
                if (IsBetween(DateTime.Now, new TimeSpan(22, 0, 0), new TimeSpan(5, 0, 0)))
                {
                    bool toggleStatus = bool.Parse(messageParts[4]);
                    IsOn = toggleStatus;
                }
            }
        }

        public static bool IsBetween(DateTime now, TimeSpan start, TimeSpan end)
        {
            TimeSpan time = now.TimeOfDay;
            // Scenario 1: If the start time and the end time are in the same day.
            if (start <= end)
                return time >= start && time <= end;
            // Scenario 2: The start time and end time is on different days.
            return time >= start || time <= end;
        }

        private void Toggle() => IsOn = !IsOn;
    }
}
