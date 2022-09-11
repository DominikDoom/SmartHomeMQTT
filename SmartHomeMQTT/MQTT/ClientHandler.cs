using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHomeMQTT.MQTT
{
    public static class ClientHandler
    {
        private static IMqttClient _client;
        public static IMqttClient Client => _client ??= Init();

        private static readonly List<string> SubscribedTopics = new();

        public static event EventHandler<MqttApplicationMessageReceivedEventArgs> MessageReceivedEvent;

        private static IMqttClient Init()
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

        public static async Task Connect()
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

        public static async Task Subscribe(string topic)
        {
            if (!Client.IsConnected)
                await Connect();

            SubscribedTopics.Add(topic);

            // Subscribe to the provided topic
            MqttTopicFilterBuilder fb = new MqttTopicFilterBuilder()
                .WithAtMostOnceQoS();
            SubscribedTopics.ForEach((sensorTopic) => fb.WithTopic(sensorTopic));

            MqttClientSubscribeOptions mqttSubscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(fb.Build())
                .Build();

            _ = await Client?.SubscribeAsync(mqttSubscribeOptions);
            Debug.WriteLine($"MQTT client subscribed to topic {topic}");
        }

        public static async Task Publish(string topic, string message)
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
