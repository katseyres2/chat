using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Server
{
    public static class TcpStatus
    {
        public const string Success = "/success";
        public const string Error = "/error";
        public const string Notification = "/notification";
    }

    public static class ErrorMessage
    {
        public const string UserAlreadyExists       = TcpStatus.Error + " UserAlreadyExists";
        public const string UserNotFound            = TcpStatus.Error + " UserNotFound";
        public const string GuestNotFound           = TcpStatus.Error + " GuestNotFound";
        public const string WrongCredentials        = TcpStatus.Error + " WrongCredentials";
        public const string InvalidParameter        = TcpStatus.Error + " InvalidParameter";
        public const string NameAlreadyExists       = TcpStatus.Error + " NameAlreadyExists";
        public const string AlreadySigned           = TcpStatus.Error + " AlreadySigned";
        public const string RoomNotJoined           = TcpStatus.Error + " RoomNotJoined";
        public const string RoomAccessDenied        = TcpStatus.Error + " RoomAccessDenied";
        public const string Error                   = TcpStatus.Error + " Error";
        public const string NotConnected            = TcpStatus.Error + " NotConnected";
        public const string PermissionDenied        = TcpStatus.Error + " PermissionDenied";
    }


    class Server {
        public const int LocalPort = 8888;
        public const string LocalHost = "0.0.0.0";
        private readonly List<User> users = new() { };
        private readonly List<ChatRoom> chatRooms = new() { };

        public Server() {
            users.Add(new User("admin", "1234"));
            users.Add(new User("max", "1234"));
            chatRooms.Add(new ChatRoom("spring", users[0]));
            chatRooms.Add(new ChatRoom("winter", users[0]));
            chatRooms.Add(new ChatRoom("summer", users[1]));

            TcpListener server = new TcpListener(IPAddress.Parse(LocalHost), LocalPort);
			server.Start();
			Console.WriteLine($"Server started, listening on {LocalHost}:{LocalPort}...");

			while (true) {
				TcpClient inboundClient = server.AcceptTcpClient();

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

        private void FindUserByTcp(TcpClient client, ref User? user)
        {
            foreach (User? u in users)
            {
                if (u.HasSameTcpClientInstance(client))
                {
                    user = u;
                    return;
                }
            }

            user = null;
        }

		private void Handler(TcpClient client)
		{
            byte[] bytes;
            string source,message,response,command,address;
            int i,port;
            User? user;
            NetworkStream stream;
            MethodInfo? meth;

            // client ip address
            address = (client.Client.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "";
            // client port
            port = (client.Client.RemoteEndPoint as IPEndPoint)?.Port ?? -1;
            
            // stream client connection
            stream = client.GetStream();
            Console.WriteLine($"New connection : {address}:{port}");

            do
            {
                bytes = new byte[256];
                response = "/";

                // read the incoming stream data
                try { i = stream.Read(bytes, 0, bytes.Length); }
                catch (IOException)
                {
                    // close everything if there is an exception
                    stream.Close();
                    client.Close();
                    
                    user = null;
                    FindUserByTcp(client, ref user);
                    user?.UnbindClient(client);
                    
                    Console.WriteLine("Client closed");
                    break;
                }

                // decode message
                source = Encoding.UTF8.GetString(bytes);

                if (source.Length == 0)
                {
                    // the message is empty
                }

                if (!isCommand(source))
                {
                    // the message doesn't start with a "/"
                }

                Console.WriteLine($"The server receives request: {source}");
                message = source.ToLower().Substring(1, source.Length - 1);
                message = Regex.Replace(message, @"\b[ ]{2,}\b", " ");

                command = message.Split(' ')[0].ToLower();

                Console.Write("Command arguments : [");
                foreach (string e in message.Split(" "))
                {
                    Console.Write($"{e} ");
                }
                Console.Write("]\n");


                foreach (var method in typeof(Server).GetMethods())
                {
                    if (command.CompareTo(method.Name.ToLower()) == 0)
                    {
                        meth = typeof(Server).GetMethod(method.Name);
                        response = (string)(meth?.Invoke(this, new object[] { client, message }) ?? "");
                    }
                }

                Console.WriteLine($"The server sends response: {response}");

                if (response.CompareTo("/") == 0) Broadcast(client, source);

                // encode message
                byte[] msg = Encoding.UTF8.GetBytes(response);
                // send response to client
                stream.Write(msg, 0, msg.Length);
            } while (i != 0);
        }

        private bool isCommand(string value)
        {
            return value.Length > 0 && value[0].CompareTo('/') == 0;
        }

        public string Disconnect(TcpClient cli, string message)
        {
            cli.GetStream().Close();
            cli.Close();
            return "";
        }

        public static string Ping(TcpClient cli, string message)
        {
            return TcpStatus.Success;
        }

        public string CreateRoom(TcpClient cli, string message)
        {
            User? user = null;
            ChatRoom? room = null;
         
            string[] args = message.Trim().Split(' ');
            if (args.Length != 2) return ErrorMessage.InvalidParameter;

            string name = args[1];

            if (!Regex.Match(name, "/b^[0-9a-zA-Z]*$/b").Success) return ErrorMessage.InvalidParameter;

            FindRoomByName(name, ref room);
            if (room != null) return ErrorMessage.NameAlreadyExists;            

            FindUserByTcp(cli, ref user);
            if (user == null) return ErrorMessage.UserNotFound;
            
            room = new(name, user);
            chatRooms.Add(room);

            Broadcast(cli, $"The room \"{name}\" appears.");
            return TcpStatus.Success;
        }

        public string ShowRooms(TcpClient cli, string message)
        {
            string output;
            User? user = null;

            string[] args = message.Trim().Split(' ');
            if (args.Length != 1) return ErrorMessage.InvalidParameter;

            FindUserByTcp(cli, ref user);
            if (user == null) return ErrorMessage.UserNotFound;

            output = TcpStatus.Success;

            foreach (ChatRoom r in chatRooms)
            {
                if (r.HasUser(user))
                {
                    output += $" {r.Name}";
                }
            }

            return output;
        }

        //public static string Delete(TcpClient cli, Server server, string message)
        //{
        //    return "i";
        //}

        //public static string Update(TcpClient cli, Server server, string message)
        //{
        //    return "i";
        //}

        private void FindRoomByName(string name, ref ChatRoom? room)
        {
            foreach (ChatRoom cr in chatRooms)
            {
                if (cr.Name == name)
                {
                    room = cr;
                    return;
                }
            }

            room = null;
        }

        public string Join(TcpClient cli, string message)
        {
            User? user = null;
            string[] args = message.Trim().Split(' ');
            if (args.Length != 2) return ErrorMessage.InvalidParameter;
            string roomName = args[1];
            FindUserByTcp(cli, ref user);

            if (user == null) return ErrorMessage.UserNotFound;

            foreach (ChatRoom cr in chatRooms)
            {
                if (cr.Name.CompareTo(roomName) == 0 && cr.HasUser(user))
                {
                    user.SwitchRoom(cr);
                    cr.Broadcast();
                    break;
                }
            }

            return TcpStatus.Success;
        }

        /// <summary>
        /// If there is a user with this username, the function stores it into the ref parameter [user].
        /// </summary>
        /// <param name="username"></param>
        /// <param name="user"></param>
        private void FindUserByName(string username, ref User? user)
        {
            foreach (User u in users)
            {
                Console.WriteLine($"{username}:{u.Username} = {u.Username.CompareTo(username) == 0}");
                if (u.Username.CompareTo(username) == 0)
                {
                    user = u;
                    return;
                }
            }

            user = null;
        }

        public string Invite(TcpClient cli, string message)
        {
            User? user = null;
            User? guest = null;

            string[] args = message.Trim().Split(' ');
            if (args.Length != 2) return ErrorMessage.InvalidParameter;

            string guestUsername = args[1];

            FindUserByTcp(cli, ref user);
            FindUserByName(guestUsername, ref guest);

            if (user == null) return ErrorMessage.UserNotFound;
            if (guest == null) return ErrorMessage.GuestNotFound;

            if (user.CurrentRoom == null) return ErrorMessage.RoomNotJoined;

            user.CurrentRoom.Add(user, guest);

            foreach (ChatRoom cr in chatRooms)
            {
                if (cr == user.CurrentRoom)
                {
                    if (cr.Add(user, guest)) return ErrorMessage.Error;
                }
            }

            return TcpStatus.Success;
        }

        //public static string Kick(TcpClient cli, Server server, string message)
        //{
        //    return "i";
        //}

        // "/signup john password123"
        private string SignUp(TcpClient cli, string message)
        {

            string output;
            bool alreadyExists = false;

            string[] args = message.Trim().Split(' ');
            if (args.Length != 3) return "Invalid arguments.";

            string username = args[1];
            string password = args[2];

            foreach (User? u in users)
            {
                if (u.Username == username)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists)
            {
                User user = new User(username, password);
                user.BindClient(cli);
                users.Add(user);
                output = $"Successful user creation, connected as {username}.";
            }
            else
            {
                output = "The username already exists.";
            }

            return output;
        }

        // "/signin john password123"
        public string Signin(TcpClient cli, string message)
        {
            string[] args = message.Trim().Split(' ');
            
            if (args.Length != 4) return ErrorMessage.InvalidParameter;

            string username = args[1];
            string password = args[2];
            int port= int.Parse(args[3]);

            User? user = null;
            FindUserByTcp(cli, ref user);

            Console.WriteLine($"User found with this TCP : '{user?.Username}', message : 'message'");

            if (user != null) return ErrorMessage.AlreadySigned;

            FindUserByName(username, ref user);
            Console.WriteLine($"username: '{username}', user: '{user}'");
            if (user == null) return ErrorMessage.UserNotFound;
            if (!user.HasSameCredentials(username, password)) return ErrorMessage.WrongCredentials;
            
            user.BindClient(cli);

            // client ip address
            string address = (cli.Client.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? LocalHost;

            user.BindClientListener(address, port);

            Broadcast(cli, $"Is connected.");
            Broadcast(null, $"allusercounter {AllUserCounter()}");
            Broadcast(null, $"connectedusercounter {ConnectedUserCounter()}");
            return TcpStatus.Success;
        }

        // "/signout"
        public string Signout(TcpClient cli, string message)
        {
            User? user = null;
            string[] args = message.Trim().Split(' ');
            if (args.Length != 1) return ErrorMessage.InvalidParameter;

            FindUserByTcp(cli, ref user);
            if (user == null) return ErrorMessage.NotConnected;

            user.UnbindClientListener();
            user.UnbindClient(cli);

            Console.WriteLine($"Is user connected : {user}");

            Broadcast(cli, $"Has left.");
            Broadcast(null, $"allusercounter {AllUserCounter()}");
            Broadcast(null, $"connectedusercounter {ConnectedUserCounter()}");
            return TcpStatus.Success;
        }

        public string UnbindClient(TcpClient cli, string message)
        {
            User? user = null;
            string[] args = message.Trim().Split(' ');
            if (args.Length != 1) return ErrorMessage.InvalidParameter;

            FindUserByTcp(cli, ref user);
            if (user == null) return ErrorMessage.NotConnected;

            user.UnbindClient(cli);
            return TcpStatus.Success;
        }

        private int AllUserCounter()
        {
            return users.Count;
        }

        private int ConnectedUserCounter()
        {
            int counter = 0;

            foreach (User u in users)
            {
                if (u.IsClientOpened()) counter++;
            }

            return counter;
        }

        public void Broadcast(TcpClient? source, string message)
        {
            User? userSource = null;
            string sender;

            if (source != null) FindUserByTcp(source, ref userSource);

            foreach (User u in users)
            {
                if (u.IsClientListenerOpened() && u != userSource)
                {
                    sender = userSource?.Username ?? "server";
                    Console.WriteLine($"The servers broadcasts to {u.Username} : {TcpStatus.Notification} {sender} {message}");
                    u.SendToClientListener($"{TcpStatus.Notification} {sender} {message}");
                }
            }
        }

        public string NewUser(TcpClient cli, string message)
        {
            User? modifier = null;
            User? user = null;
            string[] args = message.Trim().Split(' ');
            if (args.Length != 3) return ErrorMessage.InvalidParameter;

            string username = args[1];
            string password = args[2];

            FindUserByTcp(cli, ref modifier);
            if (modifier== null) return ErrorMessage.UserNotFound;
            if (!modifier.IsAdmin()) return ErrorMessage.PermissionDenied;

            FindUserByName(username, ref user);
            if (user != null) return ErrorMessage.UserAlreadyExists;

            users.Add(new User(username, password));
            Broadcast(cli, $"Has created the user {username}.");
            return TcpStatus.Success;
        }

        public string SetPassword(TcpClient cli, string message)
        {
            User? user = null;
            string[] args = message.Trim().Split(' ');

            if (args.Length != 3) return ErrorMessage.InvalidParameter;

            string username = args[1];
            string password = args[2];

            FindUserByTcp(cli, ref user);
            if (user == null) return ErrorMessage.UserNotFound;
            if (!user.IsAdmin()) return ErrorMessage.PermissionDenied;

            FindUserByName(username, ref user);
            if (user == null) return ErrorMessage.UserNotFound;

            user.SetPassword(password);
            
            return TcpStatus.Success;
        }
    }
}