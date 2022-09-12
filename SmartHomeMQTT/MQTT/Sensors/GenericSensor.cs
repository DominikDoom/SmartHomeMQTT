using SmartHomeMQTT.Utils;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartHomeMQTT.MQTT.Sensors
{
    public abstract class GenericSensor : INotifyPropertyChanged
    {
        public Guid Id { get; }
        public string Room { get; }
        public string Name { get; }
        public string Topic { get; }
        public ClientHandler ClientHandler { get; }

        public GenericSensor(Guid id, string room, string type, string name)
        {
            Id = id;
            Room = room;
            Name = name;
            Topic = TopicString.Create(Room, type, Id.ToString());
            ClientHandler = new();

            SubscribeToCentralTopic();
        }
        private async void SubscribeToCentralTopic()
        {
            await ClientHandler.Connect();
            ClientHandler.MessageReceivedEvent += HandleMessageReceived;
            await ClientHandler.Subscribe(TopicString.TOPIC_COMM);
        }

        public abstract void HandleMessageReceived(object sender, MQTTnet.Client.MqttApplicationMessageReceivedEventArgs e);

        public abstract void PublishStatus();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new(propertyName));
    }
}
