using SmartHomeMQTT.Utils;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MQTTnet.Client;

namespace SmartHomeMQTT.MQTT.Sensors
{
    /// <summary>
    /// Generic sensor implementation containing shared properties &amp; functionality.
    /// <br />
    /// Implements <see cref="INotifyPropertyChanged"/> to enable data binding to sensor values.
    /// </summary>
    public abstract class GenericSensor : INotifyPropertyChanged
    {
        /// <summary>
        /// The sensor's ID, used for generating a unique topic.
        /// </summary>
        public Guid Id { get; }
        /// <summary>
        /// The room in which the sensor is located.
        /// </summary>
        public string Room { get; }
        /// <summary>
        /// The sensor's display name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The unique topic string used for the sensor's respective
        /// <see cref="PublishStatus"/> implementation
        /// </summary>
        public string Topic { get; }
        /// <summary>
        /// The sensor's client handler wrapping an instance of
        /// <see cref="IMqttClient"/>. Used for independent publishing &amp; subscription.
        /// </summary>
        public ClientHandler ClientHandler { get; }

        /// <summary>
        /// Constructor for GenericSensor. Since the class is abstract, it isn't used directly
        /// and only forces the concrete sensor implementations to call it.
        /// </summary>
        /// <param name="id">The sensor id</param>
        /// <param name="room">The room the sensor is in</param>
        /// <param name="type">The type of the sensor ("window", "thermo", or "outlet")</param>
        /// <param name="name">The display name of the sensor</param>
        public GenericSensor(Guid id, string room, string type, string name)
        {
            Id = id;
            Room = room;
            Name = name;
            Topic = TopicString.Create(Room, type, Id.ToString());
            ClientHandler = new();

            SubscribeToCentralTopic();
        }

        /// <summary>
        /// Connects the sensor-owned client with the broker and subscribes to the global topic
        /// to listen to UI requests.
        /// </summary>
        private async void SubscribeToCentralTopic()
        {
            await ClientHandler.Connect();
            ClientHandler.MessageReceivedEvent += HandleMessageReceived;
            await ClientHandler.Subscribe(TopicString.TOPIC_COMM);
        }

        /// <summary>
        /// Abstract handler for the <see cref="ClientHandler.MessageReceivedEvent"/>.
        /// </summary>
        /// <param name="sender">The event sender, usually null here</param>
        /// <param name="e">The event parameters containing the message</param>
        public abstract void HandleMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e);

        /// <summary>
        /// Abstract handler for the status publishing functionality of the respective sensors.
        /// </summary>
        public abstract void PublishStatus();


        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// NotifyPropertyChanged implementation. Uses <see cref="CallerMemberNameAttribute"/>
        /// for convenience.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new(propertyName));
    }
}
