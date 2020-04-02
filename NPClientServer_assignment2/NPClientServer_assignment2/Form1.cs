using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NPClientServer_assignment2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<TcpClient> clients = new List<TcpClient>(); //ToolBar store connected clients
        List<CheckBox> boxes = new List<CheckBox>();    // TO store checkboxes for each clients
        TcpListener server;
        TcpClient activeClient;
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            server = new TcpListener(IPAddress.Parse("192.168.0.86"), 8001); //My Servers IP
            server.Start(10);
            Thread t2 = new Thread(AcceptClient);
            t2.Start();
        }

        //Accept clients

        public void AcceptClient()
        {
            while (true)
            {
                TcpClient c = server.AcceptTcpClient();
                clients.Add(c);
                CheckBox b = checkBoxMaker(clients.Count.ToString());
                this.BeginInvoke((Action)(() =>
                {
                    //perform on the UI thread
                    this.clientPanel.Controls.Add(b);
                }));
                activeClient = c;
                Thread t = new Thread(asd => ReadMessage(c));
                t.Start();
            }
        }


        //To Read Messages
        public void ReadMessage(TcpClient client)
        {
            while (true)
            {
                TcpClient cur = clients.Single((x) => x == client);
                NetworkStream stream = client.GetStream();
                StreamReader sdr = new StreamReader(stream);
                string msg = sdr.ReadLine();
                textBox2.AppendText(Environment.NewLine);
                textBox2.AppendText("Client" + (clients.IndexOf(cur)+1) + ": "  + msg);
            }
        }

        ///send message
        private void sendToAll(object sender, EventArgs e)
        {
            foreach (var item in clients)
            {
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("Me: " + textBox3.Text);
                NetworkStream stream = item.GetStream();
                StreamWriter sdr = new StreamWriter(stream);
                sdr.WriteLine(textBox3.Text);
                sdr.Flush();
            }
        }


        //Send Message to Selected Client
        private void sendToActiveClient(object sender, EventArgs e)
        {
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("Me: " + textBox3.Text);
                NetworkStream stream = activeClient.GetStream();
                StreamWriter sdr = new StreamWriter(stream);
                sdr.WriteLine(textBox3.Text);
                sdr.Flush();
        }


        

        //Sends message to all client
        public CheckBox checkBoxMaker(string name)
        {
            CheckBox chkBox = new CheckBox();
            chkBox.AutoSize = true;
            chkBox.Checked = true;
            chkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            //this.grpRbtn.Location = new System.Drawing.Point(279, 334);
            chkBox.Name = "CBtn"+name;
            chkBox.Size = new System.Drawing.Size(101, 17);
            chkBox.TabIndex = 22;
            chkBox.Text = "Client" + name;
            chkBox.UseVisualStyleBackColor = true;
            boxes.Add(chkBox);
            return chkBox;
        }

        //A check to determine weither to send messages to a group of clients or a single client
        private void button1_Click(object sender, EventArgs e)
        {
            if (clients.Count == 0)
                MessageBox.Show("No client(s) selected", "Select CheckBoxes", MessageBoxButtons.OK);
            else if (grpRbtn.Checked)
                sendToAll(sender,e);
            else
            {
                for (int i = 0; i < boxes.Count; i++)
                {
                    if (boxes[i].Checked)
                    {
                        activeClient = clients[i];
                        sendToActiveClient(sender, e);
                    }
                }
            }
           
            //if (grpRbtn.Checked)
            //    sendToAll(sender, e);
            //else
            //    sendToActiveClient(sender,e);
        }

       
    }
}
