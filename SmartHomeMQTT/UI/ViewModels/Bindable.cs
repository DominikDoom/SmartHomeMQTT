using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SmartHomeMQTT.UI.ViewModels
{
    /// <summary>
    /// Data Binding class wrapping the NotifyPropertyChanged implementation
    /// into a convenient to use one-liner and eliminating the need to manually create backing fields
    /// </summary>
    public abstract class Bindable : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> _properties = new();

        /// <summary>
        /// Gets the value of a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected dynamic Get([CallerMemberName] string name = null)
        {
            Debug.Assert(name != null, "name != null");
            return _properties.TryGetValue(name, out object value)
                ? value ?? default
                : default;
        }

        /// <summary>
        /// Sets the value of a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <remarks>Use this overload when implicitly naming the property</remarks>
        protected void Set<T>(T value, [CallerMemberName] string name = null)
        {
            Debug.Assert(name != null, "name != null");

            if (Equals(value, Get(name)))
                return;

            _properties[name] = value;
            OnPropertyChanged(name);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new(propertyName));
    }
}
