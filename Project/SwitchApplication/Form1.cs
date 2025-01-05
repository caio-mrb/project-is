using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using System.Text;
using uPLibrary.Networking.M2Mqtt.Messages;
using SwitchApplication.Service;

namespace SwitchApplication
{
    public partial class Form1 : Form
    {
        MqttClient mClient = new MqttClient(IPAddress.Parse("127.0.0.1")); //OR use the broker hostname
        string[] topics = { "light_bulb" };

        public Form1()
        {
            InitializeComponent();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
               
        }

        private void MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Message);
            string topic = e.Topic;

            MessageBox.Show($"Message received on topic '{topic}': {message}");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mClient.Connect(Guid.NewGuid().ToString());
            if (!mClient.IsConnected)
            {
                MessageBox.Show("Error connecting to message broker...");
                return;
            }
            MessageBox.Show("Conected to broker");

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (mClient.IsConnected)
            {
                mClient.Unsubscribe(topics);
                mClient.Disconnect();
                MessageBox.Show("Disconected");
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mClient.IsConnected)
            {
                mClient.Unsubscribe(topics);
                mClient.Disconnect();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ApiIntegration apiIntegration = new ApiIntegration();
            _ = apiIntegration.SendRecordAsync("on");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ApiIntegration apiIntegration = new ApiIntegration();
            _ = apiIntegration.SendRecordAsync("off");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
