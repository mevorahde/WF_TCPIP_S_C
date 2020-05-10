using SimpleTCP;
using System.Net;
using System;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Xsl;
using System.Xml;

namespace TCPIPDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SimpleTcpServer server;
        private void Form1_Load(object sender, EventArgs e)
        {
            server = new SimpleTcpServer();
            server.Delimiter = 0x13; //enter
            //server.StringEncoder = Encoding.UTF8;
            server.StringEncoder = Encoding.ASCII;
            server.DelimiterDataReceived += Server_DataReceived;
            labelServer.Visible = false;
        }

        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {

            string xmlString = @"<?xml version='1.0' encoding='UTF-8'?>
                                <xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0'>
	                            <xsl:param name='ControlId' />
	                            <xsl:param name='MessageTimeStamp' />
	                            <xsl:template match='/'>
		                            <XCSData>
			                            <ACK>
				                            <Success>true</Success>
				                            <ControlId>
					                            <xsl:value-of select='$ControlId' />
				                            </ControlId>
				                            <MessageTimeStamp>
					                            <xsl:value-of select='$MessageTimeStamp' />
				                            </MessageTimeStamp>
				                            <Message>Message Successfully Processed</Message>
			                           </ACK>
		                            </XCSData>
	                            </xsl:template>
                              </xsl:stylesheet>";
            try
            {
                txtStatus.Invoke((MethodInvoker)delegate ()
                {

                    //DateTime MessageTimeStamp = DateTime.Now;
                    //string recievedstring = e.MessageString;
                    var document = new XmlDocument();
                    //document.LoadXml(recievedstring);
                    //var messageID = document.GetElementsByTagName("xsl:messageid");
                    var xDoc = XDocument.Parse(xmlString);
                    string xmlDoc = xDoc.ToString();

                    //string responseBack = "Message Successful";
                    txtStatus.Text += e.MessageString;
                    //e.ReplyLine(string.Format(responseBack));
                    e.ReplyLine(string.Format(xmlDoc));
                });
            }
            catch (Exception ex)
            {
                string exError = ex.ToString();
                e.ReplyLine(string.Format(exError));
            };
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            labelServer.Visible = true;
            IPAddress ip = IPAddress.Parse(txtHost.Text);
            server.Start(ip, Convert.ToInt32(txtPort.Text));
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (server.IsStarted)
                server.Stop();
            labelServer.Visible = false;
        }
    }
}
