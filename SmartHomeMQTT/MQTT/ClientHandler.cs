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
    public class ClientHandler
    {
        private IMqttClient _client;
        public  IMqttClient Client => _client ??= Init();

        //private static readonly List<string> SubscribedTopics = new();

        public event EventHandler<MqttApplicationMessageReceivedEventArgs> MessageReceivedEvent;

        private IMqttClient Init()
        {
            MqttFactory mqttFactory = new();

            IMqttClient mClient = mqttFactory.CreateMqttClient();

            // Setup message handling before connecting so that queued messages
            // are also handled properly. When there is no event handler attached all
            // received messages get lost.
            //mqttClient.ApplicationMessageReceivedAsync += topicListener;
            mClient.ApplicationMessageReceivedAsync += e =>
            {
                Debug.WriteLine("Received application message:");
                Debug.WriteLine(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                MessageReceivedEvent.Invoke(null, e);

                return Task.CompletedTask;
            };

            return mClient;
        }

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
            MqttClientConnectResult response = await Client.ConnectAsync(mqttClientOptions, CancellationToken.None);
            Debug.WriteLine($"The MQTT client is connected.");
        }

        public async Task Subscribe(string topic)
        {
            if (!Client.IsConnected)
                await Connect();

            //SubscribedTopics.Add(topic);

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

        public async Task Publish(string topic, string message)
        {
            if (!Client.IsConnected)
                await Connect();

            MqttApplicationMessage applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(message)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                .Build();

            await Client?.PublishAsync(applicationMessage, CancellationToken.None);
            Debug.WriteLine("MQTT client published message");
        }
    }
}
