﻿using SmartHomeMQTT.Utils;
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

        public GenericSensor(Guid id, string room, string type, string name)
        {
            Id = id;
            Room = room;
            Name = name;
            Topic = TopicString.Create(Room, type, Id.ToString());
        }

        public abstract void PublishStatus();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new(propertyName));
    }
}
