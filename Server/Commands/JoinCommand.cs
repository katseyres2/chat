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
    internal class JoinCommand : Command
    {
        public JoinCommand() : base("/join", "Join a chatroom.") { }

        public override void Execute(TcpClient client, string message)
        {
            User? user = null;
            string[] args = message.Trim().Split(' ');
            
            if (args.Length != 2)
            {
                Models.Server.SendToUser(client, ErrorMessage.InvalidParameter);
                return;
            }

            string roomName = args[1];
            DiscoveryService.FindUserByTcp(client, ref user);

            if (user == null)
            {
                Models.Server.SendToUser(client, ErrorMessage.UserNotFound);
                return;
            }

            ChatRoom? room = null;
            DiscoveryService.FindRoomByName(roomName, ref room);
            
            if (room == null)
            {
                Models.Server.SendToUser(client, ErrorMessage.RoomNotFound);
                return;
            }
            
            //Models.Server.SendToChatRoom(client, room, "has joined");
            user.SwitchRoom(room);
        }
    }
}
