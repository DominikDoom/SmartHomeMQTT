using MQTTnet.Client;
using SmartHomeMQTT.Utils;
using System;
using System.Text;

namespace SmartHomeMQTT.MQTT.Sensors
{
    /// <summary>
    /// Sensor representing an electrical outlet.
    /// </summary>
    public class OutletSensor : GenericSensor
    {
        private bool _isOn = true;
        /// <summary>
        /// Represents if the outlet is turned on or off.
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
                PublishStatus();
            }
        }

        /// <summary>
        /// Constructor for OutletSensor.
        /// </summary>
        /// <param name="id">The sensor id</param>
        /// <param name="room">The room the sensor is in</param>
        /// <param name="name">The sensor's display name</param>
        public OutletSensor(Guid id, string room, string name) : base(id, room, "outlet", name) { }

        /// <summary>
        /// Publishes the on/off status of the outlet.
        /// Payload will be <c>"True"</c> for on and <c>"False"</c> for off.
        /// </summary>
        public override void PublishStatus() =>
            _ = ClientHandler.Publish(Topic, IsOn.ToString());

        /// <summary>
        /// Handles the <see cref="ClientHandler.MessageReceivedEvent"/>.
        /// <br/>
        /// Depending on the payload, the action will be one of the following:
        /// <list type="bullet">
        /// <item>Toggle on/off</item>
        /// <item>Turn on if a window in the same room opened between 22:00 and 05:00</item>
        /// </list>
        /// </summary>
        /// <param name="sender">The event sender, usually null here</param>
        /// <param name="e">The event parameters containing the message</param>
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

        /// <summary>
        /// Checks if the provided time is between a start and end time.
        /// Also handles cases where start and end are on different days.
        /// </summary>
        /// <param name="now">The time to check if contained in the range</param>
        /// <param name="start">The range start time</param>
        /// <param name="end">The range end time</param>
        /// <returns></returns>
        public static bool IsBetween(DateTime now, TimeSpan start, TimeSpan end)
        {
            TimeSpan time = now.TimeOfDay;
            // Scenario 1: If the start time and the end time are in the same day.
            if (start <= end)
                return time >= start && time <= end;
            // Scenario 2: The start time and end time is on different days.
            return time >= start || time <= end;
        }

        /// <summary>
        /// Toggles the outlet on/off
        /// </summary>
        private void Toggle() => IsOn = !IsOn;
    }
}
