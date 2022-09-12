using SmartHomeMQTT.MQTT.Sensors;
using SmartHomeMQTT.Utils;
using System;
using Xunit;

namespace SmartHomeMqttTests
{
    /// <summary>
    /// Basic tests for the generic sensor checking for correct setup.
    /// </summary>
    public class GenericSensorTests
    {
        private readonly GenericSensor G;

        // Runs before each test
        public GenericSensorTests()
        {
            // Sensor setup
            Guid id = Guid.NewGuid();
            string room = "testroom";
            string name = "generic";
            G = new WindowSensor(id, room, name);
        }

        /// <summary>
        /// Tests if ClientHandler and client are created correctly.
        /// </summary>
        [Fact(DisplayName = "Client setup")]
        public void TestClientSetup()
        {
            Assert.NotNull(G.ClientHandler);
            Assert.NotNull(G.ClientHandler.Client);
        }

        /// <summary>
        /// Tests if the sensor-specific topic string is created correctly.
        /// </summary>
        [Fact(DisplayName = "Topic string creation")]
        public void TestTopicString()
        {
            // Expected topic string
            string expectedTopic = TopicString.Create(G.Room, "window", G.Id.ToString());

            Assert.Equal(expectedTopic, G.Topic);
        }
    }
}
