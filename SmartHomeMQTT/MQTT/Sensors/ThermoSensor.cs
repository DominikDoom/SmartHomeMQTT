using MQTTnet.Client;
using SmartHomeMQTT.Utils;
using System;
using System.Globalization;
using System.Text;

namespace SmartHomeMQTT.MQTT.Sensors
{
    /// <summary>
    /// Sensor representing a radiator thermostat.
    /// </summary>
    public class ThermoSensor : GenericSensor
    {
        private bool _isOn = true;
        /// <summary>
        /// Whether the thermostat is on or off.
        /// </summary>
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
        /// <summary>
        /// Whether the thermostat is currently set to the low temperature
        /// as a reaction to an open window.
        /// </summary>
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
        /// <summary>
        /// The low temperature the sensor will regulate to if a window is open.
        /// </summary>
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
        /// <summary>
        /// The high temperature the sensor will regulate to if no window is open.
        /// </summary>
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

        /// <summary>
        /// The current temperature, derived from the current high/low mode
        /// and their respective temperatures.
        /// <br/>
        /// If the thermostat is turned off,
        /// the temperature will be displayed as 0.
        /// </summary>
        public int CurrentTemp =>
            IsOn
            ? IsLowTemp
                ? LowTemp
                : HighTemp
            : 0;

        /// <summary>
        /// Constructor for ThermoSensor.
        /// </summary>
        /// <param name="id">The sensor id</param>
        /// <param name="room">The room the sensor is in</param>
        /// <param name="name">The sensor's display name</param>
        public ThermoSensor(Guid id, string room, string name) : base(id, room, "thermo", name) { }

        /// <summary>
        /// Publishes status of the thermostat.
        /// Payload will be, comma-separated and in order:
        /// <list type="number">
        /// <item>True/False for the On/Off status</item>
        /// <item>True/False for the Low/High setting status</item>
        /// <item>Int for the low temperature</item>
        /// <item>Int for the high temperature</item>
        /// </list>
        /// </summary>
        public override void PublishStatus() =>
            _ = ClientHandler.Publish(Topic, string.Join(",", IsOn, IsLowTemp, LowTemp, HighTemp));

        /// <summary>
        /// Handles the <see cref="ClientHandler.MessageReceivedEvent"/>.
        /// <br/>
        /// Depending on the payload, the action will be one of the following:
        /// <list type="bullet">
        /// <item>Toggle on/off</item>
        /// <item>Set new high temperature</item>
        /// <item>Regulate temperature to low setting if a window in the same room opened</item>
        /// <item>Regulate temperature to high setting if a window in the same room closed</item>
        /// </list>
        /// </summary>
        /// <param name="sender">The event sender, usually null here</param>
        /// <param name="e">The event parameters containing the message</param>
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

        /// <summary>
        /// Toggles the thermostat on/off
        /// </summary>
        public void Toggle() => IsOn = !IsOn;
    }
}
