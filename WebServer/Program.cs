using System;

namespace WebServer
{
    internal class Program
    {
        private static void Main()
        {
            Server server = new Server(80, 10);
            Logger logger = new Logger();

            server.Start();

            logger.Log("Server is running...");
            Console.ReadLine();
        }
    }
}
