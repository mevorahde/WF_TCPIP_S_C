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

            DateTime timeNow = DateTime.Now;
            string MessageTimeStamp = timeNow.ToString("yyyy-MM-dd HH:mm:ss");
            string messageCheck = e.MessageString;
            if (messageCheck.StartsWith("<"))
            {
                var xDoc = XDocument.Parse(e.MessageString);

                var ns = xDoc.Root.Name.Namespace;
                var messageid = xDoc.Element(ns + "XCSData").Element(ns + "message").Element(ns + "messageid");
                XDocument xml = new XDocument(
                                        new XElement("XCSData",
                                            new XElement("NACK",
                                                         new XElement("Success", "true"),
                                                         new XElement("ControlId", messageid),
                                                         new XElement("MessageTimeStamp", MessageTimeStamp)
                                                         )
                                                    )
                                 );

                string xmlString = xml.ToString();
                try
                {
                    txtStatus.Invoke((MethodInvoker)delegate ()
                    {

                        txtStatus.Text += e.MessageString;
                        e.ReplyLine(string.Format(xmlString));
                    });
                }
                catch (Exception ex)
                {
                    string exError = ex.ToString();
                    XDocument xmlError = new XDocument(
                        new XElement("XCSData",
                            new XElement("NACK",
                                         new XElement("Success", "false"),
                                         new XElement("ControlId", messageid),
                                         new XElement("MessageTimeStamp", MessageTimeStamp),
                                         new XElement("Error", exError)
                                         )
                                    )
                 );

                    string xmlStringError = xmlError.ToString();
                    e.ReplyLine(string.Format(xmlStringError));
                };
            }
            else
            {
                Guid generatedGUID = Guid.NewGuid();

                XDocument xml = new XDocument(
                                        new XElement("XCSData",
                                            new XElement("ACK",
                                                         new XElement("Success", "true"),
                                                         new XElement("ControlId", generatedGUID),
                                                         new XElement("MessageTimeStamp", MessageTimeStamp)
                                                         )
                                                    )
                                 );

                string xmlString = xml.ToString();
                try
                {
                    txtStatus.Invoke((MethodInvoker)delegate ()
                    {

                        txtStatus.Text += e.MessageString;
                        e.ReplyLine(string.Format(xmlString));
                    });
                }
                catch (Exception ex)
                {
                    string exError = ex.ToString();
                    XDocument xmlError = new XDocument(
                        new XElement("XCSData",
                            new XElement("ACK",
                                         new XElement("Success", "false"),
                                         new XElement("ControlId", generatedGUID),
                                         new XElement("MessageTimeStamp", MessageTimeStamp),
                                         new XElement("Error", exError)
                                         )
                                    )
                 );

                    string xmlStringError = xmlError.ToString();
                    e.ReplyLine(string.Format(xmlStringError));
                };

            }

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                labelServer.Visible = true;
                IPAddress ip = IPAddress.Parse(txtHost.Text);
                server.Start(ip, Convert.ToInt32(txtPort.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occurred...");
                Console.WriteLine(ex.ToString());
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (server.IsStarted)
            {
                server.Stop();
            }
            else if (!server.IsStarted)
            {
                MessageBox.Show("Server not started...");
            }

            labelServer.Visible = false;
        }
    }
}