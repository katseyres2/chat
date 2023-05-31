using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Server.Constants;
using Server.Services;

namespace Server.Models
{
    public class ResponseMessage
    {
        public const string SUCCESS = "/SUCCESS";
        public const string FAILED = "/FAILED";
        public const string NOTIF = "/NOTIF";
    }

    class Server
    {
        public const int LOCAL_PORT = 8888;
        public const string LOCAL_HOST = "0.0.0.0";
        public static readonly List<User> users = new() { };
        public static List<ChatRoom> chatRooms = new() { };
        public readonly List<Command> commands = new() { };
        private readonly string host;
        private readonly int port;

        private TcpListener tcpListener;

        public Server(string host, int port)
        {
            tcpListener = new TcpListener(IPAddress.Parse(host), port);
            this.host = host;
            this.port = port;
        }

        /// <summary>
        /// Populate commands List dynamically with all Command in the namespace Server.Commands.
        /// The namespace Server.Commands must only have Command inherited object.
        /// </summary>
        public void PopulateCommands()
        {
            Console.WriteLine("Start populate commands.");
            // Fetch all classes included in the namespace Server.Commands.
            Type[] q = DiscoveryService.GetTypeInNameSpace(Assembly.GetExecutingAssembly(), "Server.Commands");

            // Iterates on each class.
            q.ToList().ForEach(t =>
            {
                // Instanciate constructor.
                var cmd = (Command)Activator.CreateInstance(t)!;
                commands.Add(cmd);
                Console.WriteLine($"- {cmd.name} added.");
            });

            Console.WriteLine("Commands are filled.");
        }

        /// <summary>
        /// <br>Start the listener service. It accepts all outside connections and creates a new thread for each.</br>
        /// </summary>
        public void Run()
        {
            tcpListener.Start();
            Console.WriteLine($"Server started, listening on {host}:{port}...");

            while (true)
            {
                TcpClient inboundClient = tcpListener.AcceptTcpClient();

                try
                {
                    Thread th = new Thread(() => Handler(inboundClient));
                    th.Start();
                }
                catch (IOException)
                {
                    Console.WriteLine("client closed");
                }
                catch (SocketException)
                {
                    Console.WriteLine("socket closed");
                }
            }
        }

        /// <summary>
        /// Add users in the server list to give access to the client.
        /// </summary>
        public void PopulateData()
        {
            users.Add(new User("admin", "1234"));
            users.Add(new User("max", "1234"));
            chatRooms.Add(new ChatRoom("spring", users[0]));
            chatRooms.Add(new ChatRoom("winter", users[0]));
            var summer = new ChatRoom("summer", users[1]);
            summer.Add(users[1], users[0]);
            chatRooms.Add(summer);
            
        }

        /// <summary>
        /// <br>Handle each new connection with checks and responses.</br>
        /// </summary>
        /// <param name="client"></param>
        private void Handler(TcpClient client)
        {
            byte[] bytes;
            string source, message, response, command, address;
            int i, port;
            User? user;
            NetworkStream stream;

            // client ip address
            address = (client.Client.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "";
            // client port
            port = (client.Client.RemoteEndPoint as IPEndPoint)?.Port ?? -1;

            // stream client connection
            stream = client.GetStream();
            Console.WriteLine($"New connection : {address}:{port}");

            // start the loop which receives the messages and sends the responses.
            do
            {
                bytes = new byte[256];
                //TODO deprecated
                response = ErrorMessage.CommandNotFound;

                // read the incoming stream data
                try { i = stream.Read(bytes, 0, bytes.Length); }
                catch (IOException)
                {
                    // close everything if there is an exception
                    stream.Close();
                    client.Close();

                    user = null;
                    DiscoveryService.FindUserByTcp(client, ref user);
                    user?.UnbindClient(client);

                    Console.WriteLine("Client closed");
                    break;
                }

                // decode message
                source = Encoding.UTF8.GetString(bytes);
                Console.WriteLine($"RECEIVED : {source}");

                message = Regex.Replace(source, @"\b[ ]{2,}\b", " ");
                command = message.Split(' ')[0];

                foreach (Command cmd in commands)
                {
                    if (cmd.name.CompareTo(command) == 0)
                    {
                        cmd.Execute(client, message);
                    }
                }

                Console.WriteLine($"RESPOND : {response}");

                // encode message
                byte[] msg = Encoding.UTF8.GetBytes(response);
                // send response to client


                if (client.Connected)
                    stream.Write(msg, 0, msg.Length);
                else
                    break;
            } while (i != 0);
        }

        /// <summary>
        /// <br>Close the connection between the server(listener) and this client(client).</br>
        /// </summary>
        /// <param name="cli"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public string UnbindClient(TcpClient cli, string message)
        {
            User? user = null;
            string[] args = message.Trim().Split(' ');
            if (args.Length != 1) return ErrorMessage.InvalidParameter;

            DiscoveryService.FindUserByTcp(cli, ref user);
            if (user == null) return ErrorMessage.NotConnected;

            user.UnbindClient(cli);
            return ResponseMessage.SUCCESS;
        }

        /// <summary>
        /// Send this message to all private user rooms excepted the sender.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static void Broadcast(TcpClient? source, string message)
        {
            User? userSource = null;
            string sender;

            if (source != null) DiscoveryService.FindUserByTcp(source, ref userSource);

            foreach (User u in users)
            {
                if (u.IsClientListenerOpened() && u != userSource)
                {
                    sender = userSource?.Username ?? "server";
                    u.SendToClientListener($"{ResponseMessage.NOTIF} {u.Username} _{u.Username} {message}");
                }
            }
        }

        /// <summary>
        /// Send this message to this room.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="room"></param>
        /// <param name="message"></param>
        public static void SendToChatRoom(TcpClient? source, ChatRoom room, string message)
        {
            User? userSource = null;
            string sender;

            if (source != null) DiscoveryService.FindUserByTcp(source, ref userSource);

            foreach (User u in users)
            {
                foreach (ChatRoom cr in chatRooms)
                {
                    if (cr == room && cr.HasUser(u) && u.IsClientListenerOpened())
                    {
                        sender = userSource?.Username ?? "server";
                        u.SendToClientListener($"{ResponseMessage.SUCCESS} {sender} {room.Name} {message}");
                    }
                }
            }
        }

        /// <summary>
        /// Respond to the sender on its private room.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static void SendToUser(TcpClient? source, string message)
        {
            if (source == null) return;
            User? userSource = null;
            ChatRoom? room = null;

            DiscoveryService.FindUserByTcp(source, ref userSource);
            if (userSource == null) return;
            DiscoveryService.FindRoomByName($"_{userSource.Username}", ref room);

            userSource.SendToClientListener($"{ResponseMessage.NOTIF} {userSource.Username} {room!.Name} {message}");
        }
    }
}