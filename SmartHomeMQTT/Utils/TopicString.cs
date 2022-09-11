using System;

namespace SmartHomeMQTT.Utils
{
    public static class TopicString
    {
        private static readonly string TOPIC_BASE = $"thm/mqtt/{Guid.NewGuid()}";

        public static string Create(string room, string sensorType, string id)
            => $"{TOPIC_BASE}/{room}/{sensorType}/{id}";
    }
}
