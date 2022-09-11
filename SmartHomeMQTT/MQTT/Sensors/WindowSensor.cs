using System;

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
                PublishStatus();
            }
        }

        public WindowSensor(Guid id, string room, string name) : base(id, room, "window", name) { }

        public override void PublishStatus() =>
            _ = ClientHandler.Publish(Topic, IsOpen.ToString());

        public void Toggle() => IsOpen = !IsOpen;
    }
}
