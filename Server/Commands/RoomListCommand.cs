using System.Net.Sockets;
using Server.Constants;
using Server.Models;
using Server.Services;

namespace Server.Commands
{
    internal class RoomListCommand : Command
    {
        public RoomListCommand() : base("/roomlist", "List the available rooms.") { }

        public override void Execute(TcpClient client, string message)
        {
            string output;
            User? user = null;

            string[] args = message.Trim().Split(' ');
            if (args.Length != 1)
            {
                Models.Server.SendToUser(client, ErrorMessage.InvalidParameter);
                return;
            }

            DiscoveryService.FindUserByTcp(client, ref user);
            
            if (user == null)
            {
                Models.Server.SendToUser(client, ErrorMessage.UserNotFound);
                return;
            }

            output = ResponseMessage.SUCCESS;

            foreach (ChatRoom r in Models.Server.chatRooms)
            {
                if (r.HasUser(user))
                {
                    output += $" {r.Name}";
                }
            }

            Models.Server.SendToUser(client, output);
        }
    }
}
