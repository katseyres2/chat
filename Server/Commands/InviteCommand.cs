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
    internal class InviteCommand : Command
    {
        public InviteCommand() : base("/invite", "Invite player") { }

        public override void Execute(TcpClient client, string message)
        {
            User? user = null;
            User? guest = null;

            string[] args = message.Trim().Split(' ');
            if (args.Length != 2) Models.Server.SendToUser(client, ErrorMessage.InvalidParameter);

            string guestUsername = args[1];

            DiscoveryService.FindUserByTcp(client, ref user);
            DiscoveryService.FindUserByName(guestUsername, ref guest);

            if (user == null)
            {
                Models.Server.SendToUser(client, ErrorMessage.UserNotFound);
                return;
            }

            if (guest == null)
            {
                Models.Server.SendToUser(client, ErrorMessage.GuestNotFound);
                return;
            }

            if (user.CurrentRoom == null)
            {
                Models.Server.SendToUser(client, ErrorMessage.RoomNotJoined);
                return;
            } 

            user.CurrentRoom.Add(user, guest);

            foreach (ChatRoom cr in Models.Server.chatRooms)
            {   
                if (cr == user.CurrentRoom)
                {
                    if (cr.Add(user, guest))
                    {
                        Models.Server.SendToUser(client, ErrorMessage.Error);
                    }
                }
            }

            Models.Server.SendToUser(client, "user invited !");
        }
    }
}
