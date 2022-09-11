using System;

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

        public void Toggle() => IsOn = !IsOn;
    }
}
