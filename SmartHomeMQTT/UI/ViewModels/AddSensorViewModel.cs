using SmartHomeMQTT.MQTT.Sensors;
using SmartHomeMQTT.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SmartHomeMQTT.UI.ViewModels
{
    /// <summary>
    /// ViewModel for AddSensorDialog.xaml
    /// </summary>
    public class AddSensorViewModel : Bindable
    {
        /// <summary>
        /// Event called when the save button is clicked.
        /// Expects the new sensor to return as an argument.
        /// </summary>
        public static event EventHandler<GenericSensor> SaveEvent;
        /// <summary>
        /// Event called when the cancel button is clicked.
        /// </summary>
        public static event EventHandler CancelEvent;

        /// <summary>
        /// List of possible sensor types the user can choose from.
        /// <br/>
        /// Bound to a ComboBox.
        /// </summary>
        public static IEnumerable<SensorType> SensorTypeValues
            => (IEnumerable<SensorType>)Enum.GetValues(typeof(SensorType));

        /// <summary>
        /// The currently selected sensor type.
        /// <br/>
        /// Bound to a ComboBox.
        /// </summary>
        public SensorType SelectedType { get => Get(); set => Set(value); }

        /// <summary>
        /// The room name of the sensor.
        /// <br/>
        /// Bound to a TextBox.
        /// </summary>
        public string Room
        {
            get => Get();
            set
            {
                Set(value);
                _saveCommand?.RaiseCanExecuteChanged();
            }
        }
        /// <summary>
        /// The display name of the sensor.
        /// <br/>
        /// Bound to a TextBox.
        /// </summary>
        public string Name
        {
            get => Get();
            set
            {
                Set(value);
                _saveCommand?.RaiseCanExecuteChanged();
            }
        }

        public AddSensorViewModel()
        {
            SelectedType = SensorType.WINDOW;
        }

        private DelegateCommand _saveCommand;
        /// <summary>
        /// The command for saving.
        /// Only enabled when all text fields are non-empty
        /// </summary>
        public ICommand SaveCommand => _saveCommand ??= new(Save, CanSave);

        private DelegateCommand _cancelCommand;
        /// <summary>
        /// The command for cancelling.
        /// </summary>
        public ICommand CancelCommand => _cancelCommand ??= new(Cancel);

        /// <summary>
        /// Saves the selected sensor type, the room, and the name in a new sensor object
        /// and passes it to the <see cref="SaveEvent"/> invocation.
        /// </summary>
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
        /// <summary>
        /// Determines if the save button is active.
        /// All text fields need to be filled with non-whitespace for it to be true.
        /// </summary>
        /// <returns>Whether save can be called or not</returns>
        private bool CanSave() => Room?.Trim().Length > 0 && Name?.Trim().Length > 0;

        /// <summary>
        /// Invokes <see cref="CancelEvent"/>
        /// </summary>
        private static void Cancel() => CancelEvent?.Invoke(null, null);
    }
}
