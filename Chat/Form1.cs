using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Chat
{
    public partial class Form1 : Form
    {
        readonly Client client;
        public static List<string> remoteMessageQueue = new List<string> { };

        public Form1()
        {
            InitializeComponent();
            client = new Client();
            Application.Idle += HandleApplicationIdle;
        }

        void HandleApplicationIdle(object sender, EventArgs e)
        {
            string message;
            string username;
            string[] parameters;
            int index;

            while (remoteMessageQueue.Count > 0)
            {
                index = remoteMessageQueue.Count - 1;
                message = remoteMessageQueue[index];

                if (message.Contains("allusercounter")) updateAllUserCounter(int.Parse(message.Split(' ')[3]));
                else if (message.Contains("connectedusercounter")) updateConnectedUserCounter(int.Parse(message.Split(' ')[3]));
                else
                {
                    // "/notification user Hello World !
                    parameters = message.Split(' ');
                    username = parameters[1];
                    message = message.Replace(username, "").Replace("/notification", "").Trim();
                    displayServerMessage(username, message);
                }

                remoteMessageQueue.RemoveAt(remoteMessageQueue.Count - 1);
            }
        }

        private void updateAllUserCounter(int counter)
        {
            label24.Text = counter.ToString();
        }

        private void updateConnectedUserCounter(int counter)
        {
            label23.Text = counter.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string message;
            string response;
            string username = textBox4.Text;
            string password = textBox5.Text;
            int localPort = int.Parse(textBox7.Text);

            if (button4.Text.ToLower().CompareTo("sign in") == 0)
            {
                Console.WriteLine("Start signin");
                // start the client listener before sending to server
                // that he can open a new TCP client to create the bound
                client.StartListen(localPort);
                Console.WriteLine($"Listen on {localPort}");
                // then send credentials with the port the client is listening with.
                message = $"/signin {username} {password} {localPort}";
                response = client.Send(message);
                Console.WriteLine($"Sent {message}");
                label12.Text = response;

                if (response ==  TcpStatus.Success)
                {
                    label7.Text = username;
                    EnableUser();
                    DisableAuthenticationForm();
                    textBox5.Text = textBox4.Text = "";
                    button4.Text = "Sign Out";
                    if (username.CompareTo("admin") == 0) EnableAdmin();
                    displayServerMessage(null, $"Signed as {username}.");
                }

                Console.WriteLine("End signin");
            }
            else
            {
                // send to the server that he can stop its TcpClient for the local listener
                response = client.Send($"/signout");
                // then stop the local listener
                client.StopListen();
                
                label12.Text = response;
                displayServerMessage(null, "Signed out.");
                button4.Text = "Sign in";
                label7.Text = "";
                
                clearInputs();
                DisableConfigurationPanel();
                EnableAuthenticationForm();
            }
        }

        private void EnableConnectionForm()
        {
            textBox2.Enabled = textBox1.Enabled = textBox7.Enabled = true;
        }

        private void DisableConnectionForm()
        {
            textBox2.Enabled = textBox1.Enabled = textBox7.Enabled = false;
        }


        private void DisableAuthenticationForm()
        {
            textBox5.Enabled = textBox4.Enabled
                = button1.Enabled
                = false;
        }

        private void EnableAuthenticationForm()
        {
            textBox5.Enabled = textBox4.Enabled
                = button1.Enabled
                = true;
        }

        private void EnableUser()
        {
            textBox6.Enabled = button3.Enabled
                = textBox3.Enabled
                = button2.Enabled
                = label6.Enabled
                = true;
        }

        private void EnableAdmin()
        {
            textBox9.Enabled = label17.Enabled
                = label16.Enabled
                = label5.Enabled
                = textBox8.Enabled
                = label15.Enabled
                = button8.Enabled
                = label18.Enabled
                = textBox10.Enabled
                = button9.Enabled
                = label19.Enabled
                = label20.Enabled
                = textBox11.Enabled
                = true;
        }

        private void DisableConfigurationPanel()
        {
            textBox6.Enabled = button3.Enabled
                = textBox9.Enabled
                = label17.Enabled
                = label16.Enabled
                = label5.Enabled
                = textBox8.Enabled
                = textBox3.Enabled
                = button2.Enabled
                = button8.Enabled
                = label15.Enabled
                = label6.Enabled
                = label18.Enabled
                = textBox10.Enabled
                = button9.Enabled
                = label19.Enabled
                = label20.Enabled
                = textBox11.Enabled
                = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string address = textBox1.Text;
            int port = int.Parse(textBox2.Text);

            if (button1.Text.CompareTo("Connect") == 0)
            {
                // connect the client to the server on [address:port]
                client.Connect(address, port);
                displayServerMessage(null, "Connected.");
                DisableConnectionForm();
            }
            else
            {
                // you must send to server to close the notification bound
                client.Send("/unbindclient");
                // then you can close the client
                client.Disconnect();
                
                displayServerMessage(null, "Disconnected.");
                EnableConnectionForm();
            }
            
            button7.Enabled = button4.Enabled = textBox5.Enabled = textBox4.Enabled = !textBox4.Enabled;

            if (button1.Text.CompareTo("Connect") == 0)
            {
                label4.Text = "connected";
                label4.ForeColor = Color.Green;
                button1.Text = "Disconnect";
            }
            else
            {
                label4.Text = "disconnected";
                label4.ForeColor = Color.Red;
                button1.Text = "Connect";
                clearInputs();
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            textBox5.PasswordChar = '\u25CF';
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            string msg = textBox3.Text;

            if (msg.Length > 0)
            {
                textBox3.Text = "";
                msg = msg.TrimStart('/');
                client.Send(msg);
                displayServerMessage("you",msg);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 13)
            {
                button2_MouseDown(sender, null);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (client.Send("/ping") == TcpStatus.Success) displayServerMessage(null,"Pong !");
        }

        public void displayServerMessage(string username, string msg)
        {
            if (username == null) username = "server";
            if (username.Length > 15) username = username.Substring(0, 15);

            ListViewItem lvi = new ListViewItem(new string[] {DateTime.Now.ToShortTimeString(), username, msg}, -1);
            if (username == "server") lvi.ForeColor = Color.Orange;
            listView1.Items.Insert(0, lvi);
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            string response;
            string username = textBox8.Text;
            string password = textBox9.Text;

            textBox8.Text = textBox9.Text = "";

            if (username.Length == 0 || password.Length == 0)
            {
                return;
            }

            response = client.Send($"/newuser {username} {password}");

            if (response == TcpStatus.Success) displayServerMessage(null,$"User {username} created.");
            else if (response == ErrorMessage.UserAlreadyExists) displayServerMessage(null,$"User {username} already exists.");
        }

        private void clearInputs()
        {
            textBox3.Text = textBox4.Text
                = textBox5.Text = textBox6.Text
                = textBox8.Text = textBox9.Text = textBox10.Text
                = textBox11.Text = "";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string username = textBox10.Text;
            string password = textBox11.Text;

            if (username.Length != 0 && password.Length != 0) client.Send($"/setpassword {username} {password}");
        }
    }
}
