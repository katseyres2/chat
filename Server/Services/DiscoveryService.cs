using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Services
{
    internal class DiscoveryService
    {
        public static void FindUserByTcp(TcpClient client, ref User? user)
        {
            foreach (User? u in Models.Server.users)
            {
                if (u.HasSameTcpClientInstance(client))
                {
                    user = u;
                    return;
                }
            }

            user = null;
        }

        public static void FindRoomByName(string name, ref ChatRoom? room)
        {
            foreach (ChatRoom cr in Models.Server.chatRooms)
            {
                if (cr.Name == name)
                {
                    room = cr;
                    return;
                }
            }

            room = null;
        }

        /// <summary>
        /// If there is a user with this username, the function stores it into the ref parameter [user].
        /// </summary>
        /// <param name="username"></param>
        /// <param name="user"></param>
        public static void FindUserByName(string username, ref User? user)
        {
            foreach (User u in Models.Server.users)
            {
                Console.WriteLine($"{username}:{u.Username} = {u.Username.CompareTo(username) == 0}");
                if (u.Username.CompareTo(username) == 0)
                {
                    user = u;
                    return;
                }
            }

            user = null;
        }

        public static int AllUserCounter()
        {
            return Models.Server.users.Count;
        }

        public static int ConnectedUserCounter()
        {
            int counter = 0;

            foreach (User u in Models.Server.users)
            {
                if (u.IsClientOpened()) counter++;
            }

            return counter;
        }

        public static Type[] GetTypeInNameSpace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }
    }
}
