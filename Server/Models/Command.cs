using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Server.Interfaces;

namespace Server.Models
{
    /// <summary>
    /// <br>You can build a command in the namespace "Server.Commands" and the server will handle it dynamically.</br>
    /// <br>The command has a name and its documentation.</br>
    /// <br>You can execute command with the method object.Exectute(cli, message).</br>
    /// </summary>
    internal abstract class Command : ICommand
    {
        public string name;
        public string documentation;

        public Command(string name, string documentation)
        {
            this.name = name;
            this.documentation = documentation;
        }

        /// <summary>
        /// Execute the command instructions.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        public abstract void Execute(TcpClient client, string message);

        public override string ToString()
        {
            return $"{name} : {documentation}";
        }
    }
}
