using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class User
    {
        // the socket through the client is connected
        private TcpClient? client;
        // the socket to send a notification to the client
        private TcpClient? clientListener;
        private readonly string username;
        private string password;
        private ChatRoom? currentRoom;

        public string Username {get { return username; }}
        public ChatRoom? CurrentRoom { get { return currentRoom; } }
        public NetworkStream? Stream { get { return client?.GetStream(); } }

        public bool HasSameCredentials(string username, string password)
        {
            return username.CompareTo(this.username) == 0 && password.CompareTo(this.password) == 0;
        }

        public string SendToClient(string message)
        {
            if (client != null)
            {
                byte[] data = Encoding.ASCII.GetBytes(message);
                client.GetStream().Write(data, 0, data.Length);
                data = new byte[2048];
                int bytes = client.GetStream().Read(data, 0, data.Length);
                return Encoding.ASCII.GetString(data, 0, bytes);
            }
            else
            {
                return "The connection is closed";
            }
        }

        public string SendToClientListener(string message)
        {
            if (IsClientListenerOpened())
            {
                byte[] data = Encoding.ASCII.GetBytes(message);
                clientListener!.GetStream().Write(data, 0, data.Length);
                data = new byte[2048];
                int bytes = clientListener.GetStream().Read(data, 0, data.Length);
                return Encoding.ASCII.GetString(data, 0, bytes);
            }
            else
            {
                return "The connection is closed";
            }
        }

        public bool IsAdmin()
        {
            return username.CompareTo("admin") == 0;
        }

        /// check if the client is connected to this user
        public bool HasSameTcpClientInstance(TcpClient client)
        {
            return client == this.client;
        }

        /// create a new user
        public User(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        /// <summary>
        /// bind a client to this user
        /// </summary>
        /// <param name="cli"></param>
        public void BindClient(TcpClient cli)
        {
            if (IsClientOpened()) return;
            client = cli;
        }

        /// <summary>
        /// unbind the user client socket
        /// </summary>
        /// <param name="c"></param>
        public void UnbindClient(TcpClient c)
        {
            if (client != c) return;
            client = null;
        }

        /// <summary>
        /// create a client listener instance for this user
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public void BindClientListener(string address, int port)
        {
            if (IsClientListenerOpened()) return;
            try { clientListener = new TcpClient(address, port); }
            catch (Exception e) { Console.WriteLine(e); }
        }

        /// <summary>
        /// stop the client listener instance for this user
        /// </summary>
        public void UnbindClientListener()
        {
            if (!IsClientListenerOpened()) return;

            clientListener!.Close();
            clientListener = null;
        }

        /// <summary>
        /// check if there is an opened session for this user
        /// </summary>
        /// <returns></returns>
        public bool IsClientOpened()
        {
            return client != null && client.Connected;
        }

        /// <summary>
        /// check if the client can receive notifications
        /// </summary>
        /// <returns></returns>
        public bool IsClientListenerOpened()
        {
            return clientListener != null && clientListener.Connected;
        }

        public bool SwitchRoom(ChatRoom cr)
        {
            if (cr.HasUser(this))
            {
                currentRoom = cr;
                return true;
            }

            return false;
        }

        public void SetPassword(string newPassword)
        {
            password = newPassword;
        }
    }
}
