using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Server.Constants;
using Server.Models;
using Server.Services;

namespace Server.Commands
{
    internal class SignInCommand : Command
    {
        public SignInCommand() : base("/signin", "Authenticate as a existing user.") { }

        public override void Execute(TcpClient client, string message)
        {
            string[] args = message.Trim().Split(' ');

            if (args.Length != 4)
            {
                Models.Server.SendToUser(client, ErrorMessage.InvalidParameter);
                return;
            }

            string username = args[1];
            string password = args[2];
            int port = int.Parse(args[3]);

            User? user = null;
            DiscoveryService.FindUserByTcp(client, ref user);

            Console.WriteLine($"User found with this TCP : '{user?.Username}', message : 'message'");

            if (user != null)
            {
                Models.Server.SendToUser(client, ErrorMessage.AlreadySigned);
                return;
            }

            DiscoveryService.FindUserByName(username, ref user);
            Console.WriteLine($"username: '{username}', user: '{user}'");

            if (user == null)
            {
                Models.Server.SendToUser(client, ErrorMessage.UserNotFound);
                return;
            }

            if (!user.HasSameCredentials(username, password))
            {
                Models.Server.SendToUser(client, ErrorMessage.WrongCredentials);
                return;
            }

            user.BindClient(client);

            // client ip address
            string address = (client.Client.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? Models.Server.LOCAL_HOST;

            user.BindClientListener(address, port);

            Models.Server.Broadcast(client, $"Is connected.");
            Models.Server.Broadcast(null, $"allusercounter {DiscoveryService.AllUserCounter()}");
            Models.Server.Broadcast(null, $"connectedusercounter {DiscoveryService.ConnectedUserCounter()}");
        }
    }
}
