using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Commands
{
    internal class PingCommand : Command
    {
        public PingCommand() : base("/ping", "Ping the server.") { }

        public override void Execute(TcpClient client, string message)
        {
            Models.Server.SendToUser(client, message);
        }
    }
}
