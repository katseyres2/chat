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

            if (user.CurrentRoom.HasUser(guest))
            {
                Models.Server.SendToUser(client, ErrorMessage.UserAlreadyJoined);
                return;
            }

            if (!user.CurrentRoom.IsAdmin(user))
            {
                Models.Server.SendToUser(client, ErrorMessage.PermissionDenied);
                return;
            }

            user.CurrentRoom.Add(user, guest);
            Models.Server.SendToUser(client, $"The user {guest.Username} has joined the room.");

            if (guest.IsClientOpened())
                Models.Server.SendToUser(guest.GetTcpClient(), $"room {user.CurrentRoom.Name}");
        }
    }
}
