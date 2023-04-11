namespace Server
{
    internal class ChatRoom
    {
        private User admin;
        private string name;
        private readonly List<User> users = new() { };
        //private readonly List<Message> messages = new () { };

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

        public bool IsAdmin(User user)
        {
            return user == admin;
        }

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

        public bool SetName(User admin, string newName)
        {
            if (IsAdmin(admin))
            {
                name = newName;
                return true;
            }

            return false;
        }

        public bool Add(User admin, User guest)
        {
            if (IsAdmin(admin) && !HasUser(guest))
            {
                users.Add(guest);
                return true;
            }

            return false;
        }

        public bool Kick(User admin, User guest)
        {
            if (IsAdmin(admin) && HasUser(guest))
            {
                users.Remove(guest);
                return true;
            }

            return false;
        }

        public bool TransferOwnership(User oldAdmin, User newAdmin)
        {
            if (IsAdmin(oldAdmin))
            {
                admin = newAdmin;
                return true;
            }

            return false;
        }

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

        //public bool send(Message message)
        //{
        //    foreach ()
        //}
    }
}
