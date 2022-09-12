using System;

namespace SmartHomeMQTT.Utils
{
    /// <summary>
    /// Utility class to get global or sensor-specific topic strings.
    /// </summary>
    public static class TopicString
    {
        /// <summary>
        /// Base topic string to build on for sensor-specific topics
        /// </summary>
        private static readonly string TOPIC_BASE = $"thm/mqtt/{Guid.NewGuid()}";
        /// <summary>
        /// Global topic listened to by all sensors
        /// </summary>
        public static readonly string TOPIC_COMM = $"thm/mqttc/{Guid.NewGuid()}";

        /// <summary>
        /// Creates a sensor-specific unique topic using its parameters.
        /// </summary>
        /// <param name="room">The room the sensor is in</param>
        /// <param name="sensorType">The type of the sensor ("window", "thermo", or "outlet")</param>
        /// <param name="id">The sensor id</param>
        /// <returns></returns>
        public static string Create(string room, string sensorType, string id)
            => $"{TOPIC_BASE}/{room}/{sensorType}/{id}";
    }
}
