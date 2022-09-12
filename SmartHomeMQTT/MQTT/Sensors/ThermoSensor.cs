using MQTTnet.Client;
using SmartHomeMQTT.Utils;
using System;
using System.Globalization;
using System.Text;

namespace SmartHomeMQTT.MQTT.Sensors
{
    public class ThermoSensor : GenericSensor
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
                OnPropertyChanged(nameof(CurrentTemp));
                PublishStatus();
            }
        }

        private bool _isLowTemp;
        public bool IsLowTemp
        {
            get => _isLowTemp;
            set
            {
                if (value == _isLowTemp)
                    return;

                _isLowTemp = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentTemp));
                PublishStatus();
            }
        }

        private int _lowTemp = 12;
        public int LowTemp
        {
            get => _lowTemp;
            set
            {
                if (value == _lowTemp)
                    return;

                _lowTemp = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentTemp));
            }
        }

        private int _highTemp = 22;
        public int HighTemp
        {
            get => _highTemp;
            set
            {
                if (value == _highTemp)
                    return;

                _highTemp = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentTemp));
                PublishStatus();
            }
        }

        public int CurrentTemp =>
            IsOn
            ? IsLowTemp
                ? LowTemp
                : HighTemp
            : 0;

        public ThermoSensor(Guid id, string room, string name) : base(id, room, "thermo", name) { }

        public override void PublishStatus() =>
            _ = ClientHandler.Publish(Topic, string.Join(",", IsOn, IsLowTemp, LowTemp, HighTemp));

        public override void HandleMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            if (e.ApplicationMessage.Topic != TopicString.TOPIC_COMM)
                return;

            string[] messageParts = Encoding.UTF8.GetString(e.ApplicationMessage.Payload).Split(",");

            // Toggle on/off
            if (messageParts[2] == Id.ToString() && messageParts[3] == "toggle")
                Toggle();

            // Change temperature
            if (messageParts[2] == Id.ToString()
                && messageParts[3] == "settemp")
            {
                HighTemp = int.Parse(messageParts[4], NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            // A window in the same room was opened, set temp to the low setting
            if (messageParts[0] == Room
            && messageParts[1] == "window"
            && messageParts[3] == "toggle")
            {
                bool isOpen = bool.Parse(messageParts[4]);
                IsLowTemp = isOpen;
            }
        }

        public void Toggle() => IsOn = !IsOn;
    }
}
