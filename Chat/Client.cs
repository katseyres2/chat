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

        /// <summary>
        /// Connect your object to the server on the specific.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="serverPort"></param>
        public void Connect(string server, int serverPort)
        {
            client = new TcpClient(server, serverPort);
        }

        /// <summary>
        /// Break the bound between your client service and the server.
        /// It only stops one direction (client -> server).
        /// </summary>
        public void Disconnect()
        {
            Send("/disconnect");
            client.GetStream().Close();
            client.Close();
            client = null;
        }

        /// <summary>
        /// Stop your listener service.
        /// </summary>
        public void StopListen()
        {
            listener.Stop();
            listenerThread.Abort();
            listener = null;
        }

        /// <summary>
        /// Send ping request.
        /// </summary>
        public void Ping()
        {
            Send("/ping");
        }

        /// <summary>
        /// Open a port to receive all data from outside.
        /// You can receive here messages pushed from server to you.
        /// </summary>
        /// 
        /// <param name="port"></param>
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

        /// <summary>
        /// Send a request to the server.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string Send(string message)
        {
            Console.WriteLine($"SEND : {message}");
            if (client != null)
            {
                Console.WriteLine(1);
                byte[] data = Encoding.ASCII.GetBytes(message);
                client.GetStream().Write(data, 0, data.Length);
                Console.WriteLine(2);
                data = new byte[4096];
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

        public void Kick(string username)
        {
            Send($"/kick {username}");
        }

        /// <summary>
        /// Send authentication request.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void SignIn(string username, string password, int localPort)
        {
            Send($"/signin {username} {password} {localPort}");
        }

        /// <summary>
        /// Send subscription request.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void SignUp(string username, string password)
        {
            Send($"/signup {username} {password}");
        }

        /// <summary>
        /// Send disconnection request.
        /// </summary>
        public void SignOut()
        {
            Send($"/signout");
        }

        /// <summary>
        /// Send this message to this room.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="message"></param>
        public void SendToRoom(string room, string message)
        {
            Console.WriteLine($"BEFORE SEND : {message}");
            Send($"/sendtoroom {room} {message}");
        }

        /// <summary>
        /// Ask to create a new user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void NewUser(string username, string password)
        {
            Send($"/newuser {username} {password}");
        }

        /// <summary>
        /// Ask to receive all users stored on the server.
        /// </summary>
        public void UserList()
        {
            Send($"/userlist");
        }

        /// <summary>
        /// Invite a user in the room you currently are.
        /// </summary>
        /// <param name="username"></param>
        public void Invite(string username)
        {
            Send($"/invite {username}");
        }

        /// <summary>
        /// Ask to update the user password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        public void SetPassword(string username, string newPassword)
        {
            Send($"/setpassword {username} {newPassword}");
        }

        /// <summary>
        /// Ask to create a new room.
        /// </summary>
        /// <param name="room"></param>
        public void NewRoom(string room)
        {
            Send($"/newroom {room}");
        }

        /// <summary>
        /// Ask to join this room
        /// </summary>
        /// <param name="room"></param>
        public void Join(string room)
        {
            Send($"/join {room}");
        }

        /// <summary>
        /// Ask to get all room you can access.
        /// </summary>
        public void RoomList()
        {
            Send($"/roomList");
        }
    }
}
