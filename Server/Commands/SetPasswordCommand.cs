using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Server.Constants;
using Server.Models;
using Server.Services;

namespace Server.Commands
{
    internal class SetPasswordCommand : Command
    {
        public SetPasswordCommand() : base("/setpassword", "Set a new password.") { }

        public override void Execute(TcpClient client, string message)
        {
            User? user = null;
            string[] args = message.Trim().Split(' ');

            if (args.Length != 3)
            {
                Models.Server.SendToUser(client, ErrorMessage.InvalidParameter);
                return;
            }

            string username = args[1];
            string password = args[2];

            DiscoveryService.FindUserByTcp(client, ref user);

            if (user == null)
            {
                Models.Server.SendToUser(client, ErrorMessage.UserNotFound);
                return;
            }

            if (!user.IsAdmin())
            {
                Models.Server.SendToUser(client, ErrorMessage.PermissionDenied);
                return;
            }

            DiscoveryService.FindUserByName(username, ref user);

            if (user == null)
            {
                Models.Server.SendToUser(client, ErrorMessage.UserNotFound);
                return;
            }

            user.SetPassword(password);
            Models.Server.SendToUser(client, "password updated");
        }
    }
}
