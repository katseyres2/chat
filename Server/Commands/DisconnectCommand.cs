using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Commands
{
    internal class DisconnectCommand : Command
    {
        public DisconnectCommand() : base("/disconnect", "Disconnect client.") { }

        public override void Execute(TcpClient client, string message)
        {
            client.GetStream().Close();
            client.Close();
        }
    }
}
