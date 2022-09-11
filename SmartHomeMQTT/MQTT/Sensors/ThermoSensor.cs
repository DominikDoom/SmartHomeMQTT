using System;

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
                PublishStatus();
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

        public int CurrentTemp => IsLowTemp ? LowTemp : HighTemp;

        public ThermoSensor(Guid id, string room, string name) : base(id, room, "thermo", name) { }

        public override void PublishStatus() =>
            _ = ClientHandler.Publish(Topic, string.Join(",", IsOn, IsLowTemp, LowTemp, HighTemp));

        public void Toggle() => IsOn = !IsOn;
    }
}
