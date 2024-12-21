using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt;
using System.Net;
using System.Text;

namespace Api.Messaging
{

    public class MqttPublisher
    {
        private readonly MqttClient _mqttClient;
        private readonly string _brokerAddress;

        public MqttPublisher(string brokerAddress)
        {
            _brokerAddress = brokerAddress;
            _mqttClient = new MqttClient(_brokerAddress);
        }

        
        public void Connect()
        {
            this._mqttClient.Connect(Guid.NewGuid().ToString());
            if (!this._mqttClient.IsConnected)
            {
                return;
            }
        }

        public void PublishMessage(string topic, string message)
        {
            try
            {
                if (this._mqttClient.IsConnected)
                {
                    this._mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao publicar mensagem: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            if (_mqttClient.IsConnected)
            {
                _mqttClient.Disconnect();
                Console.WriteLine("Desconectado do broker MQTT.");
            }
        }
    }
}
