using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Server.Interfaces;

namespace Server.Models
{
    internal abstract class Command : ICommand
    {
        public string name;
        public string documentation;

        public Command(string name, string documentation)
        {
            this.name = name;
            this.documentation = documentation;
        }

        public abstract void Execute(TcpClient client, string message);

        public override string ToString()
        {
            return $"{name} : {documentation}";
        }
    }
}
