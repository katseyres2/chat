using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    internal interface ICommand
    {
        public void Execute(TcpClient client, string message);
    }
}
