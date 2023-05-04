using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Server.Constants;
using Server.Models;

namespace Server.Commands
{
    internal class SignUpCommand : Command
    {
        public SignUpCommand(): base("/signup", "Create a new user and bind tcpClient and this user.") {}

        public override void Execute(TcpClient client, string message)
        { 
            string[] args = message.Trim().Split(' ');
            
            if (args.Length != 3)
            {
                Models.Server.SendToUser(client, ErrorMessage.InvalidParameter);
                return;
            }

            string username = args[1];
            string password = args[2];

            foreach (User? u in Models.Server.users)
            {
                if (u.Username == username)
                {
                    Models.Server.SendToUser(client, ErrorMessage.UserAlreadyExists);
                    return;
                }
            }

            User user = new User(username, password);
            user.BindClient(client);
            Models.Server.users.Add(user);
            Models.Server.SendToUser(client, $"Successful user creation, connected as {username}.");
        }
    }
}
