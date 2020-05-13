using SimpleTCP;
using System.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SimpleTcpClient client;
        private void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            labelClient.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new SimpleTcpClient();
            //client.StringEncoder = Encoding.UTF8;
            client.StringEncoder = Encoding.ASCII;
            client.DelimiterDataReceived += Client_DataReceived;
            labelClient.Visible = false;
            labelResponse.Visible = false;
        }

        private void Client_DataReceived(object sender, SimpleTCP.Message e)
        {
            txtStatus.Invoke((MethodInvoker)delegate ()
            {
                txtStatus.Text += e.MessageString;
                //string responseBack = System.Text.Encoding.ASCII.GetString(Data, 0, bytes).Trim();
                //txtStatus.Text += responseBack;
                labelResponse.Visible = true;
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            client.Connect(txtHost.Text, Convert.ToInt32(txtPort.Text));
            client.WriteLineAndGetReply(txtMessage.Text, TimeSpan.FromSeconds(3));
        }
    }
}
