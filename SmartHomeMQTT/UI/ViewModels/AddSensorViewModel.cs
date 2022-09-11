using SmartHomeMQTT.MQTT.Sensors;
using SmartHomeMQTT.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SmartHomeMQTT.UI.ViewModels
{
    public class AddSensorViewModel : Bindable
    {
        public static event EventHandler<GenericSensor> SaveEvent;
        public static event EventHandler CancelEvent;

        public static IEnumerable<SensorType> SensorTypeValues
            => (IEnumerable<SensorType>)Enum.GetValues(typeof(SensorType));

        public SensorType SelectedType { get => Get(); set => Set(value); }

        public string Room
        {
            get => Get();
            set
            {
                Set(value);
                _saveCommand.RaiseCanExecuteChanged();
            }
        }
        public string Name
        {
            get => Get();
            set
            {
                Set(value);
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public AddSensorViewModel()
        {
            SelectedType = SensorType.WINDOW;
        }

        private DelegateCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new(Save, CanSave);

        private DelegateCommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new(Cancel);

        private void Save()
        {
            Guid id = Guid.NewGuid();
            GenericSensor sensor = SelectedType switch
            {
                SensorType.WINDOW => new WindowSensor(id, Room, Name),
                SensorType.THERMO => new ThermoSensor(id, Room, Name),
                SensorType.OUTLET => new OutletSensor(id, Room, Name),
                _ => throw new NotImplementedException()
            };
            SaveEvent?.Invoke(null, sensor);
        }
        private bool CanSave() => Room?.Trim().Length > 0 && Name?.Trim().Length > 0;

        private static void Cancel() => CancelEvent?.Invoke(null, null);
    }
}
