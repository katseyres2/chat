﻿using System;
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
    internal class NewUserCommand : Command
    {
        public NewUserCommand() : base("/newuser", "Create a new user.") { }

        public override void Execute(TcpClient client, string message)
        {
            User? source = null;
            User? newUser = null;
            string[] args = message.Trim().Split(' ');
            
            if (args.Length != 3)
            {
                Models.Server.SendToUser(client, ErrorMessage.InvalidParameter);
                return;
            }

            string username = args[1];
            string password = args[2];

            DiscoveryService.FindUserByTcp(client, ref source);
            
            if (source == null)
            {
                Models.Server.SendToUser(client, ErrorMessage.UserNotFound);
                return;
            }

            if (!source.IsAdmin())
            {
                Models.Server.SendToUser(client, ErrorMessage.PermissionDenied);
                return;
            }

            DiscoveryService.FindUserByName(username, ref newUser);

            if (newUser != null)
            {
                Models.Server.SendToUser(client, ErrorMessage.UserAlreadyExists);
                return;
            }

            Models.Server.users.Add(new User(username, password));
            Models.Server.Broadcast(null, $"user {username}");
        }
    }
}
