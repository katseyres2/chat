using System.Net;
using System.Net.Sockets;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        private const String username = "john";
        private const String password = "john1234";
        private const String username1 = "jane";
        private const String password1 = "jane1234";
        private const String roomName = "test";
        private const int port = 8888;
        private const IPAddress localAddress = new IPAddress;

        [TestMethod]
        public void TestChatRoom()
        {
            var user = new Server.Models.User(username, password);
            var user1 = new Server.Models.User(username1, password1);
            var cr = new Server.Models.ChatRoom(roomName, user);

            Assert.AreEqual($"name : {roomName}, admin : {username}, users : 1", cr.ToString());
            Assert.IsTrue(cr.IsAdmin(user));
            Assert.IsFalse(cr.IsAdmin(user1));
            Assert.IsTrue(cr.HasUser(user));
            Assert.IsFalse(cr.HasUser(user1));
            Assert.IsTrue(cr.SetName(user, "test1"));
            Assert.IsFalse(cr.SetName(user1, "test2"));
            Assert.AreEqual($"name : test1, admin : {username}, users : 1", cr.ToString());
            Assert.IsTrue(cr.Add(user, user1));
            Assert.IsTrue(cr.HasUser(user1));
            Assert.IsTrue(cr.TransferOwnership(user, user1));
            Assert.IsTrue(cr.IsAdmin(user1));
            Assert.IsFalse(cr.IsAdmin(user));
            Assert.IsTrue(cr.Kick(user1, user));
            Assert.IsFalse(cr.HasUser(user));
        }

        [TestMethod]
        public void TestUser()
        {
            var tcpClient = new TcpClient();
            var tcpListener = new TcpListener(localAddress, port);
            var user = new Server.Models.User(username, password);

            Assert.IsNull(user.GetTcpClient());
            Assert.AreEqual(username, user.Username);
            Assert.IsNull(user.CurrentRoom);
            Assert.IsNull(user.Stream);
            Assert.IsTrue(user.HasSameCredentials(username, password));
            Assert.IsFalse(user.HasSameCredentials(username, "wrongPassword"));
            Assert.IsFalse(user.IsAdmin());
            user.BindClient(tcpClient);

        }
    }
}