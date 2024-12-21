using SmartLightApp.Services;
using SmartLightApp.Properties;
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
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;

namespace SmartLightApp
{
    public partial class MainForm : Form
    {
        MqttClient mClient = new MqttClient(IPAddress.Parse("127.0.0.1"));
        string[] topics = { "light_bulb" };

        public MainForm()
        {
            InitializeComponent();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            InitialConfiguration config = new InitialConfiguration();
            await config.InitializeContainerAsync();

            string imagePath = Path.Combine(Application.StartupPath, "Resources", "light_off.png");
            pictureBox1.Image = Image.FromFile(imagePath);

            MessageBox.Show("Container inicial configurado com sucesso!");

            mClient.Connect(Guid.NewGuid().ToString());
            if (!mClient.IsConnected)
            {
                MessageBox.Show("Error connecting to message broker...");
                return;
            }


            mClient.MqttMsgPublishReceived += MqttMsgPublishReceived; 
            byte[] qosLevels = { 0 }; 
            mClient.Subscribe(topics, qosLevels);

            MessageBox.Show("Subscribed to topic: light_bulb");
        }

        private void MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Message).Trim();
            string topic = e.Topic;

            string folderPath = @"Resources\";

            if (message.Equals("on", StringComparison.OrdinalIgnoreCase))
            {
                string imagePath = Path.Combine(Application.StartupPath, "Resources", "light_on.png");
                pictureBox1.Image = Image.FromFile(imagePath);
            } else if (message == "off") {
                string imagePath = Path.Combine(Application.StartupPath, "Resources", "light_off.png");
                pictureBox1.Image = Image.FromFile(imagePath);
            }
            else
            {
                string imagePath = Path.Combine(Application.StartupPath, "Resources", "light_on.png");
                pictureBox1.Image = Image.FromFile(imagePath);
            }
          
            MessageBox.Show($"Message received on topic '{topic}': {message}");
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
            mClient.Connect(Guid.NewGuid().ToString());
            if (!mClient.IsConnected)
            {
                MessageBox.Show("Error connecting to message broker...");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (mClient.IsConnected)
            {
                mClient.Unsubscribe(topics);
                mClient.Disconnect();
            }
        }
    }
}
