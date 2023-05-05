using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Server.Models;
using Server.Services;
using Server.Constants;

namespace Server.Commands
{
    internal class CreateRoomCommand : Command
    {
        public CreateRoomCommand() : base("/newroom", "New room.") { }

        public override void Execute(TcpClient client, string message)
        {
            User? user = null;
            ChatRoom? room = null;

            string[] args = message.Trim().Split(' ');
            if (args.Length != 2) Models.Server.SendToUser(client, ErrorMessage.InvalidParameter);

            string name = args[1];

            // check if the room only contains specific characters
            if (!Regex.Match(name, "/b^[0-9a-zA-Z]*$/b").Success) 
            {
                Models.Server.SendToUser(client, ErrorMessage.UserAlreadyExists);
            }

            DiscoveryService.FindRoomByName(name, ref room);
            
            if (room != null)
            {
                Models.Server.SendToUser(client, ErrorMessage.NameAlreadyExists);
                return;
            }

            DiscoveryService.FindUserByTcp(client, ref user);
            
            if (user == null)
            {
                Models.Server.SendToUser(client, ErrorMessage.UserNotFound);
                return;
            }

            room = new(name, user);
            
            Models.Server.chatRooms.Add(room);
            Models.Server.SendToUser(client, $"The room \"{name}\" appears.");
        }
    }
}
