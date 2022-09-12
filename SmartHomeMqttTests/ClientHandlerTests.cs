using SmartHomeMQTT.MQTT;
using System;
using System.Text;
using Xunit;

namespace SmartHomeMqttTests
{
    /// <summary>
    /// Tests MQTT connectivity through the client handler
    /// </summary>
    public class ClientHandlerTests
    {
        private readonly ClientHandler Handler;

        // Runs before each test
        public ClientHandlerTests()
        {
            Handler = new();
        }

        /// <summary>
        /// Tests client initialization
        /// </summary>
        [Fact(DisplayName = "Client Initialization")]
        public void TestInit()
        {
            Assert.NotNull(Handler.Client);
        }

        /// <summary>
        /// Tests connecting to the HiveMq broker
        /// </summary>
        [Fact(DisplayName = "Connect to Broker")]
        public async void TestConnect()
        {
            await Handler.Connect();
            // Check if CleanSession is false
            Assert.False(Handler.Client.Options.CleanSession);
            // Check if KeepAlive is 60 seconds
            Assert.Equal(Handler.Client.Options.KeepAlivePeriod, TimeSpan.FromSeconds(60));
            // Check if successfully connected
            Assert.True(Handler.Client.IsConnected);
        }

        /// <summary>
        /// Tests publishing & subscription on the same topic
        /// </summary>
        [Fact(DisplayName = "Publishing & Subscription")]
        public async void TestPublishSubscribe()
        {
            string topic = $"thm/mqtt/tests/{Guid.NewGuid()}";

            // Connect & subscribe to the topic
            await Handler.Connect();
            await Handler.Subscribe(topic);

            string messageToSend = "This is a test message. Secret 12345!";

            // Set up received event handler
            Handler.MessageReceivedEvent += (s, e) =>
            {
                // Get payload message
                string receivedMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                // Check if sent & received are equal
                Assert.Equal(messageToSend, receivedMessage);
            };

            await Handler.Publish(topic, messageToSend);
        }
    }
}
