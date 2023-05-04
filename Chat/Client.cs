using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chat
{
    public class Client
    {
        TcpClient client;
        TcpListener listener;
        Thread listenerThread;

        private const string HOST = "0.0.0.0";

        public Client() {}

        public void Connect(string server, int serverPort)
        {
            client = new TcpClient(server, serverPort);
        }

        public void Disconnect()
        {
            Send("/disconnect");
            client.GetStream().Close();
            client.Close();
            client = null;
        }

        public void Ping()
        {
            Send("/ping");
        }

        public void StartListen(int port)
        {
            if (listener != null) return;

            listenerThread = new Thread(() => {
                int i;
                byte[] bytes = new byte[2048];
                string message;

                listener = new TcpListener(IPAddress.Parse(HOST), port);
                listener.Start();

                TcpClient cli = listener.AcceptTcpClient();
                NetworkStream stream = cli.GetStream();

                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    message = Encoding.ASCII.GetString(bytes, 0, i);
                    Form1.remoteMessageQueue.Add(message);
                    byte[] data = Encoding.ASCII.GetBytes(message);
                    cli.GetStream().Write(data, 0, data.Length);
                }
            });

            listenerThread.Start();
        }

        public void StopListen()
        {
            listener.Stop();
            Console.WriteLine(listenerThread.ThreadState);
            listenerThread.Abort();
            listener = null;
        }

        public string Send(string message)
        {
            Console.WriteLine($"SEND : {message}");
            if (client != null)
            {
                Console.WriteLine(1);
                byte[] data = Encoding.ASCII.GetBytes(message);
                client.GetStream().Write(data, 0, data.Length);
                Console.WriteLine(2);
                data = new byte[2048];
                int bytes = client.GetStream().Read(data, 0, data.Length);
                Console.WriteLine(3);
                string response = Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine($"RESPONSE : {response}");
                return response;
            }
            else
            {
                return "The connection is closed";
            }
        }

        public void SignIn(string username, string password)
        {
            Send($"/signin {username} {password}");
        }

        public void SignUp(string username, string password)
        {
            Send($"/signup {username} {password}");
        }

        public void SignOut()
        {
            Send($"/signout");
        }

        public void SendToRoom(string room, string message)
        {
            Send($"/sendtoroom {room} {message}");
        }

        public void NewUser(string username, string password)
        {
            Send($"/newuser {username} {password}");
        }

        public void UserList()
        {
            Send($"/userlist");
        }

        public void Invite(string username)
        {
            Send($"/invite {username}");
        }

        public void SetPassword(string username, string oldPassword, string newPassword)
        {
            Send($"/setpassword {username} {oldPassword} {newPassword}");
        }

        public void NewRoom(string room)
        {
            Send($"/newroom {room}");
        }

        public void Join(string room)
        {
            Send($"/join {room}");
        }

        public void RoomList()
        {
            Send($"/roomList");
        }
    }
}
