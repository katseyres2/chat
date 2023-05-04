using System.Reflection;

namespace Server
{
    internal static class Program
    {
        static void Main()
        {
            Models.Server s = new Models.Server(Models.Server.LOCAL_HOST, Models.Server.LOCAL_PORT);
            s.populateCommands();
            s.populateData();
            s.run();
        }
    }
}
