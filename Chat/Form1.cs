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

using ListView = System.Windows.Forms.ListView;

namespace Chat
{
    public partial class Form1 : Form
    {
        readonly Client client;
        public static List<string> remoteMessageQueue = new List<string> { };
        public static string SUCCESS_RESPONSE = "/";

        public Form1()
        {
            InitializeComponent();
            client = new Client();
            Application.Idle += HandleApplicationIdle;
        }

        void HandleApplicationIdle(object sender, EventArgs e)
        {
            string key, value, username, data, roomName, message;
            string[] parameters;
            int index;

            while (remoteMessageQueue.Count > 0)
            {
                index = remoteMessageQueue.Count - 1;
                data = remoteMessageQueue[index];
                parameters = data.Split(' ');

                username = parameters.Length > 1  ? parameters[1] : "";
                roomName = parameters.Length > 2  ? parameters[2] : "";
                key = parameters.Length > 3       ? parameters[3] : "";
                value = parameters.Length > 4     ? parameters[4] : "";

                Console.WriteLine($"IN : {data} ; key={key} ; value={value} ; roomName={roomName} ; username={username}");

                if (key.CompareTo("connectedusercounter") == 0)
                {
                    UpdateConnectedUserCounter(int.Parse(value));
                    label7.Text = username;
                    EnableUser();
                    DisableAuthenticationForm();
                    textBox5.Text = textBox4.Text = "";
                    button4.Text = "Sign Out";
                    
                    label8.Enabled = label14.Enabled 
                        = button6.Enabled = comboBox2.Enabled 
                        = button5.Enabled = comboBox1.Enabled 
                        = button7.Enabled = true;

                    if (username.CompareTo("admin") == 0) EnableAdmin();
                }
                else if (key.CompareTo("allusercounter") == 0) UpdateAllUserCounter(int.Parse(value));
                else if (key.CompareTo("room") == 0) AddRoom(value);
                else if (key.CompareTo("user") == 0)
                {
                    comboBox1.Items.Add(value);
                    comboBox2.Items.Add(value);
                }
                else
                {
                    message = "";

                    for (int i = 3; i < parameters.Length; i++)
                    {
                        message += $"{parameters[i]} ";
                    }

                    DisplayServerMessage(username, roomName, message);
                }


                //DisplayServerMessage(username, message);

                //if (message.Contains("allusercounter")) updateAllUserCounter(int.Parse(message.Split(' ')[3]));
                //else if (message.Contains("connectedusercounter")) updateConnectedUserCounter(int.Parse(message.Split(' ')[3]));
                //else
                //{
                //    string[] args = message.Split(' ');
                //    string roomName = args[2];

                //    foreach (TabPage tab in tabControl1.TabPages)
                //    {
                //        //if (tab.Name == )
                //    }

                //    // "/notification user Hello World !
                //    parameters = message.Split(' ');
                //    username = parameters[1];
                //    message = message.Replace(username, "").Replace("/notification", "").Trim();
                //    displayServerMessage(username, message);
                //}

                remoteMessageQueue.RemoveAt(remoteMessageQueue.Count - 1);
            }
        }

        private void AddRoom(string name)
        {
            TabPage tb = buildTabPage(name);
            tabControl1.Controls.Add(tb);
        }

        private static TabPage buildTabPage(string name)
        {
            TabPage tb = new TabPage();
            tb.Location = new Point(4, 22);
            tb.Name = name;
            tb.Padding = new Padding(3);
            tb.Size = new Size(745, 460);
            tb.TabIndex = 1;
            tb.Text = name;
            tb.UseVisualStyleBackColor = true;
            tb.Controls.Add(buildListView(name));

            return tb;
        }

        private static ListView buildListView(string name)
        {
            var hour = new ColumnHeader();
            var user = new ColumnHeader();
            var message = new ColumnHeader();

            hour.Text = "Hour";
            user.Text = "User";
            user.Width = 100;
            message.Text = "Message";
            message.Width = 580;

            var lv = new ListView();
            lv.Activation = ItemActivation.OneClick;
            lv.Alignment = ListViewAlignment.SnapToGrid;
            lv.AutoArrange = false;
            lv.BorderStyle = BorderStyle.None;
            lv.Columns.AddRange(new ColumnHeader[] { hour, user, message});
            lv.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lv.HideSelection = false;
            lv.HoverSelection = true;
            lv.Location = new Point(-4, 0);
            lv.MultiSelect = false;
            lv.Name = name;
            lv.ShowGroups = false;
            lv.Size = new Size(753, 464);
            lv.TabIndex = 1;
            lv.UseCompatibleStateImageBehavior = false;
            lv.View = View.Details;

            return lv;
        }

        private void UpdateAllUserCounter(int counter)
        {
            label24.Text = counter.ToString();
        }

        private void UpdateConnectedUserCounter(int counter)
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
            int localPort = (new Random()).Next(1234, 2000);

            if (button4.Text.ToLower().CompareTo("sign in") == 0)
            {
                Console.WriteLine("Start signin");
                // start the client listener before sending to server that he can open a new TCP client to create the bound
                client.StartListen(localPort);
                Console.WriteLine($"Listen on {localPort}");
                // then send credentials with the port the client is listening with.
                client.SignIn(username, password, localPort);
                Console.WriteLine("End signin");
            }
            else
            {
                // send to the server that he can stop its TcpClient for the local listener
                client.SignOut();
                //then stop the local listener
                client.StopListen();

                button4.Text = "Sign in";
                label7.Text = "";
                button5.Enabled = comboBox1.Enabled = button7.Enabled = false;

                clearInputs();
                DisableConfigurationPanel();
                EnableAuthenticationForm();
                RemoveRooms();
            }
        }

        private void RemoveRooms()
        {
            foreach (TabPage tb in tabControl1.TabPages)
            {
                tabControl1.TabPages.Remove(tb);
            }
        }

        //private void FetchRooms()
        //{
        //    TabPage tabPage;
        //    string response = client.Send("/roomlist");

        //    string[] args = response.Split(' ');

        //    for (int i = 1; i < args.Length; i++)
        //    {
        //        ListViewItem li = new ListViewItem(new string[] {"17:00:01","max","Hello !"}, -1);

        //        ColumnHeader newHour = new ColumnHeader();
        //        newHour.Text = "Hour";
        //        ColumnHeader newUser = new ColumnHeader();
        //        user.Text = "User";
        //        user.Width = 100;
        //        ColumnHeader newMessage = new ColumnHeader();
        //        message.Text = "Message";
        //        message.Width = 577;

        //        System.Windows.Forms.ListView lv = new System.Windows.Forms.ListView();
        //        lv.Activation = ItemActivation.OneClick;
        //        lv.Alignment = ListViewAlignment.SnapToGrid;
        //        lv.AutoArrange = false;
        //        lv.BorderStyle = BorderStyle.None;
        //        lv.Columns.AddRange(new ColumnHeader[] {newHour,newUser,newMessage});
        //        lv.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        //        lv.HideSelection = false;
        //        lv.HoverSelection = true;
        //        lv.Items.AddRange(new ListViewItem[] {li});
        //        lv.Location = new Point(-4, 0);
        //        lv.MultiSelect = false;
        //        lv.ShowGroups = false;
        //        lv.Size = new Size(753, 464);
        //        lv.TabIndex = tabControl1.TabCount + 1;
        //        lv.UseCompatibleStateImageBehavior = false;
        //        lv.View = View.Details;
        //        //lv.SelectedIndexChanged += new EventHandler((object sender, EventArgs e) => {
        //        //    Console.WriteLine(tabControl1.);
        //        //});

        //        Console.WriteLine(args[i]);
        //        tabPage = new TabPage();
        //        tabPage.Text = args[i];
        //        tabPage.Name = args[i];
        //        tabControl1.Controls.Add(tabPage);
        //        tabPage.Controls.Add(lv);
        //    }
        //}

        private void EnableConnectionForm()
        {
            textBox2.Enabled = textBox1.Enabled = true;
        }

        private void DisableConnectionForm()
        {
            textBox2.Enabled = textBox1.Enabled = false;
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
                //DisplayServerMessage(null, "Connected.");
                DisableConnectionForm();
            }
            else
            {                
                // then you can close the client
                client.Disconnect();
                //DisplayServerMessage(null, "Disconnected.");
                EnableConnectionForm();
            }
            
            button4.Enabled = textBox5.Enabled = textBox4.Enabled = !textBox4.Enabled;

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
                client.SendToRoom(tabControl1.SelectedTab.Name, msg);
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

        private static bool IsEnterKeyboard(char c)
        {
            return Convert.ToInt32(c) == 13;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            client.Ping();
            //if (client.Ping() == SUCCESS_RESPONSE) displayServerMessage(null,"Pong !");
        }

        public void DisplayServerMessage(string username, string roomName, string msg)
        {
            if (username.Length > 15) username = username.Substring(0, 15);

            ListViewItem lvi = new ListViewItem(new string[] {DateTime.Now.ToShortTimeString(), username, msg}, -1);

            foreach (TabPage tb in tabControl1.TabPages)
            {
                if (tb.Name.CompareTo(roomName) == 0)
                {

                    var elems = tb.Controls.Find(roomName, true);
                    var lv = (ListView)elems[0];
                    lv.Items.AddRange(new ListViewItem[] { lvi });
                }
            }
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
            
            client.NewUser(username, password);
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

            if (username.Length != 0 && password.Length != 0) client.SetPassword(username, password);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl control = (TabControl) sender;

            if (control.SelectedTab != null)
            {
                Console.WriteLine($"changed : {control.SelectedIndex}, {control.SelectedTab.Name}");
                client.Join(control.SelectedTab.Name);
            }
        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine($"TabIndex changed {tabControl1.TabIndex}, {tabControl1.SelectedIndex}");
        }

        private void EventCreateRoom(object sender, MouseEventArgs e)
        {
            client.NewRoom(textBox6.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            client.Invite(comboBox1.Text);
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (IsEnterKeyboard(e.KeyChar))
                EventCreateRoom(sender, null);
        }

        private void button5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (IsEnterKeyboard(e.KeyChar))
                button5_Click(sender, null);
        }

        private void button6_MouseClick(object sender, MouseEventArgs e)
        {
            client.Kick(comboBox2.Text);
        }
    }
}
