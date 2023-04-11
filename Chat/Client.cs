using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chat
{
    public static class TcpStatus
    {
        public const string Success = "/success";
        public const string Error = "/error";
        public const string Notification = "/notification";
    }

    public static class ErrorMessage
    {
        public const string UserAlreadyExists = TcpStatus.Error + " UserAlreadyExists";
        public const string UserNotFound = TcpStatus.Error + " UserNotFound";
        public const string GuestNotFound = TcpStatus.Error + " GuestNotFound";
        public const string WrongCredentials = TcpStatus.Error + " WrongCredentials";
        public const string InvalidParameter = TcpStatus.Error + " InvalidParameter";
        public const string NameAlreadyExists = TcpStatus.Error + " NameAlreadyExists";
        public const string AlreadySigned = TcpStatus.Error + " AlreadySigned";
        public const string RoomNotJoined = TcpStatus.Error + " RoomNotJoined";
        public const string Error = TcpStatus.Error + " Error";
        public const string NotConnected = TcpStatus.Error + " NotConnected";
        public const string PermissionDenied = TcpStatus.Error + " PermissionDenied";
    }

    public class Client
    {
        TcpClient client;
        TcpListener listener;
        Thread listenerThread;
        //Thread clientThread;
        //bool clientIsRunning = false;
        //Form1 form;

        private const string LocalHost = "0.0.0.0";

        public Client() {}

        public void Connect(string server, int serverPort)
        {
            client = new TcpClient(server, serverPort);
        }

        public void Disconnect()
        {
            client.GetStream().Close();
            client.Close();
            client = null;
        }

        public void StartListen(int port)
        {
            if (listener != null) return;

            listenerThread = new Thread(() => {
                int i;
                byte[] bytes = new byte[2048];
                string message;

                listener = new TcpListener(IPAddress.Parse(LocalHost), port);
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
            listenerThread.Abort();
            listener = null;
        }

        public string Send(string message)
        {
            if (client != null)
            {
                Console.WriteLine(1);
                byte[] data = Encoding.ASCII.GetBytes(message);
                client.GetStream().Write(data, 0, data.Length);
                Console.WriteLine(2);
                data = new byte[2048];
                int bytes = client.GetStream().Read(data, 0, data.Length);
                Console.WriteLine(3);
                return Encoding.ASCII.GetString(data, 0, bytes);
            }
            else
            {
                return "The connection is closed";
            }
        }
    }
}
