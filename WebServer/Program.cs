using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebServer
{
    internal class Program
    {
        private static Socket Server;
        private static List<Socket> SocketClients;
        private static readonly byte[] Buffer = new byte[1024];

        private static void Main()
        {
            int portNumber = 3000;
            int concurrentUsers = 10;

            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Server.Bind(new IPEndPoint(IPAddress.Any, portNumber));
            Server.Listen(concurrentUsers);

            SocketClients = new List<Socket>();

            AcceptConnection();

            Console.WriteLine("Server is running...");
            Console.ReadLine();
        }

        private static void AcceptConnection()
        {
            Server.BeginAccept(AcceptCallback, null);
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket client = Server.EndAccept(ar);
            SocketClients.Add(client);
            client.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, ReceiveCallback, client);
            AcceptConnection();
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;

            Console.WriteLine("User connected!");

            string htmlString = "<h1>Index</h1>";
            byte[] htmlBytes = Encoding.Unicode.GetBytes(htmlString);
            client.BeginSend(htmlBytes, 0, htmlBytes.Length, SocketFlags.None, SendCallback, client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            Socket client = (Socket) ar.AsyncState;
            client.Shutdown(SocketShutdown.Send);
        }
    }
}
