using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHomeMQTT.MQTT
{
    /// <summary>
    /// A wrapper for the MQTTnet library client providing
    /// convenience functions for this specific use case
    /// </summary>
    public class ClientHandler
    {
        private IMqttClient _client;
        /// <summary>
        /// The actual client instance used by the handler.
        /// </summary>
        public IMqttClient Client => _client ??= Init();

        /// <summary>
        /// Event triggered when a message is received on a subscribed topic.
        /// </summary>
        public event EventHandler<MqttApplicationMessageReceivedEventArgs> MessageReceivedEvent;

        /// <summary>
        /// Init function creating a <see cref="IMqttClient"/> with the right settings.
        /// </summary>
        /// <returns>The created client instance</returns>
        private IMqttClient Init()
        {
            MqttFactory mqttFactory = new();

            IMqttClient mClient = mqttFactory.CreateMqttClient();

            // Setup message handling before connecting so that queued messages
            // are also handled properly. When there is no event handler attached all
            // received messages get lost.
            mClient.ApplicationMessageReceivedAsync += e =>
            {
                Debug.WriteLine("Received application message:");
                Debug.WriteLine(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                MessageReceivedEvent?.Invoke(null, e);

                return Task.CompletedTask;
            };

            return mClient;
        }

        /// <summary>
        /// Connects the client to the public HiveMq broker.
        /// </summary>
        /// <returns>A passed-through Task object</returns>
        public async Task Connect()
        {
            // Configure options
            MqttClientOptions mqttClientOptions = new MqttClientOptionsBuilder()
                .WithWebSocketServer("broker.hivemq.com:8000/mqtt")
                .WithCleanSession(false)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
                .WithClientId(Guid.NewGuid().ToString())
                .Build();

            // Connect to the broker
            _ = await Client.ConnectAsync(mqttClientOptions, CancellationToken.None);
            Debug.WriteLine($"The MQTT client is connected.");
        }

        /// <summary>
        /// Subscribes this client to the provided topic.
        /// </summary>
        /// <param name="topic">The topic to subscribe to</param>
        /// <returns>A passed-through Task object</returns>
        public async Task Subscribe(string topic)
        {
            if (!Client.IsConnected)
                await Connect();

            // Subscribe to the provided topic
            MqttTopicFilter f = new MqttTopicFilterBuilder()
                .WithAtMostOnceQoS()
                .WithTopic(topic)
                .Build();

            MqttClientSubscribeOptions mqttSubscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(f)
                .Build();

            _ = await Client?.SubscribeAsync(mqttSubscribeOptions);
            Debug.WriteLine($"MQTT client subscribed to topic {topic}");
        }

        /// <summary>
        /// Publishes a message on the provided topic.
        /// </summary>
        /// <param name="topic">The topic to publish on</param>
        /// <param name="message">The message payload</param>
        /// <returns>A passed-through Task object</returns>
        public async Task Publish(string topic, string message)
        {
            if (!Client.IsConnected)
                await Connect();

            MqttApplicationMessage applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(message)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                .Build();

            _ = await Client?.PublishAsync(applicationMessage, CancellationToken.None);
            Debug.WriteLine("MQTT client published message");
        }
    }
}
