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
    internal class SignOutCommand : Command
    {
        public SignOutCommand() : base("/signout", "Close the user session.") { }

        public override void Execute(TcpClient client, string message)
        {
            User? user = null;
            string[] args = message.Trim().Split(' ');
            Console.WriteLine($"User1 : {user}");
            
            if (args.Length != 1)
            {
                Models.Server.SendToUser(client, ErrorMessage.InvalidParameter);
                return;
            }

            DiscoveryService.FindUserByTcp(client, ref user);
            Console.WriteLine($"User2 : {user}");
            
            if (user == null)
            {
                Models.Server.SendToUser(client, ErrorMessage.NotConnected);
                return;
            }

            Console.WriteLine(user.IsClientOpened());
            user.UnbindClientListener();
            user.UnbindClient(client);

            Console.WriteLine($"Is user connected : {user}");

            Models.Server.Broadcast(client, $"Has left.");
            Models.Server.Broadcast(null, $"allusercounter {DiscoveryService.AllUserCounter()}");
            Models.Server.Broadcast(null, $"connectedusercounter {DiscoveryService.ConnectedUserCounter()}");
        }
    }
}
