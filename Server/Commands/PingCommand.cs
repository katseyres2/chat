using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Models;
using Server.Services;

namespace Server.Commands
{
    internal class PingCommand : Command
    {
        public PingCommand() : base("/ping", "Ping the server.") { }

        public override void Execute(TcpClient client, string message)
        {
            User? user = null;
            DiscoveryService.FindUserByTcp(client, ref user);          

            Models.Server.SendToChatRoom(client, user!.CurrentRoom!,  "pong");
        }
    }
}
