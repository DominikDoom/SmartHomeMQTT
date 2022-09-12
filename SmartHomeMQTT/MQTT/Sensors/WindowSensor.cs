using MQTTnet.Client;
using SmartHomeMQTT.Utils;
using System;
using System.Text;

namespace SmartHomeMQTT.MQTT.Sensors
{
    /// <summary>
    /// Sensor representing a window.
    /// </summary>
    public class WindowSensor : GenericSensor
    {
        private bool _isOpen;
        /// <summary>
        /// Whether the window is open or closed.
        /// </summary>
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

        /// <summary>
        /// Constructor for WindowSensor.
        /// </summary>
        /// <param name="id">The sensor id</param>
        /// <param name="room">The room the sensor is in</param>
        /// <param name="name">The sensor's display name</param>
        public WindowSensor(Guid id, string room, string name) : base(id, room, "window", name) { }

        /// <summary>
        /// Publishes the open/closed status of the window.
        /// Payload will be <c>"True"</c> for open and <c>"False"</c> for closed.
        /// </summary>
        public override void PublishStatus() =>
            _ = ClientHandler.Publish(Topic, IsOpen.ToString());

        /// <summary>
        /// Handles the <see cref="ClientHandler.MessageReceivedEvent"/>.
        /// <br/>
        /// Depending on the payload, the action will be one of the following:
        /// <list type="bullet">
        /// <item>Toggle on/off</item>
        /// </list>
        /// </summary>
        /// <param name="sender">The event sender, usually null here</param>
        /// <param name="e">The event parameters containing the message</param>
        public override void HandleMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            if (e.ApplicationMessage.Topic != TopicString.TOPIC_COMM)
                return;

            string[] messageParts = Encoding.UTF8.GetString(e.ApplicationMessage.Payload).Split(",");

            if (messageParts[2] == Id.ToString() && messageParts[3] == "toggle")
                Toggle();
        }

        /// <summary>
        /// Opens / Closes the window
        /// </summary>
        private void Toggle() => IsOpen = !IsOpen;
    }
}
