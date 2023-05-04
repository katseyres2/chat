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
    internal class MessageToRoomCommand : Command
    {
        public MessageToRoomCommand() : base("/messagetochannel", "Close the user session.") { }

        public override void Execute(TcpClient client, string message)
        {
            List<string> args = message.Trim().Split(' ').ToList();
            
            if (args.Count < 3)
            {
                Models.Server.SendToUser(client, ErrorMessage.InvalidParameter);
                return;
            }
            
            string targetChannel = args[1];
            ChatRoom? room = null;
            DiscoveryService.FindRoomByName(targetChannel, ref room);

            if (room == null)
            {
                Models.Server.SendToUser(client, ErrorMessage.RoomNotFound);
                return;
            }

            string text = string.Join(" ", args.GetRange(2, args.Count - 2));
            Models.Server.SendToChatRoom(client, room, text);
        }
    }
}
