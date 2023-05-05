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
        /// <summary>
        /// Find a user who a link with this client.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
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

        /// <summary>
        /// Find a room which has this name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="room"></param>
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

        /// <summary>
        /// Count all server users.
        /// </summary>
        /// <returns></returns>
        public static int AllUserCounter()
        {
            return Models.Server.users.Count;
        }

        /// <summary>
        /// Count all connected server users.
        /// </summary>
        /// <returns></returns>
        public static int ConnectedUserCounter()
        {
            int counter = 0;

            foreach (User u in Models.Server.users)
            {
                if (u.IsClientOpened()) counter++;
            }

            return counter;
        }

        /// <summary>
        /// Return an array of classes inside of this namespace.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        public static Type[] GetTypeInNameSpace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }
    }
}
