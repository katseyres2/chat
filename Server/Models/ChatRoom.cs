namespace Server.Models
{
    /// <summary>
    /// <br>The chatroom is a group that hosts specific users.</br>
    /// <br>By default, a private room is instanciated for each user.</br>
    /// <br>Only users in this room can join it.</br>
    /// </summary>
    public class ChatRoom
    {
        private User admin;
        private string name;
        private readonly List<User> users = new() { };

        public string Name { get { return name; } }

        override
        public string ToString()
        {
            return $"name : {name}, admin : {admin.Username}, users : {users.Count}";
        }

        public ChatRoom(string name, User admin)
        {
            this.admin = admin;
            this.name = name;
            users.Add(admin);
        }

        /// <summary>
        /// Check if this user is the room administrator.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsAdmin(User user)
        {
            return user == admin;
        }

        /// <summary>
        /// Find if this is user is in this room.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool HasUser(User user)
        {
            foreach (User u in users)
            {
                if (u.Username.CompareTo(user.Username) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// <br>Update the room name (admin right).</br>
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool SetName(User admin, string newName)
        {
            if (IsAdmin(admin))
            {
                name = newName;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add a this guest in this room (admin right).
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="guest"></param>
        /// <returns></returns>
        public bool Add(User admin, User guest)
        {
            if (IsAdmin(admin) && !HasUser(guest))
            {
                users.Add(guest);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove this guest from the room (admin right).
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="guest"></param>
        /// <returns></returns>
        public bool Kick(User admin, User guest)
        {
            if (IsAdmin(admin) && HasUser(guest))
            {
                users.Remove(guest);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Change the admin user. The room can only have 1 admin. (admin right)
        /// </summary>
        /// <param name="oldAdmin"></param>
        /// <param name="newAdmin"></param>
        /// <returns></returns>
        public bool TransferOwnership(User oldAdmin, User newAdmin)
        {
            if (IsAdmin(oldAdmin))
            {
                admin = newAdmin;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Broadcast()
        {
            foreach (User u in users)
            {
                if (u.IsClientOpened())
                {
                    // Translate the passed message into ASCII and store it as a Byte array.
                    byte[] data = System.Text.Encoding.ASCII.GetBytes("hello");
                    u.Stream?.Write(data, 0, data.Length);
                }
            }
        }
    }
}
