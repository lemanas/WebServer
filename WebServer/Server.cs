using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebServer
{
    public class Server
    {
        private static Socket _server;
        private static List<Socket> _socketClients;
        private static Logger _logger;
        private static readonly byte[] Buffer = new byte[1024];

        public Server(int portNumber, int concurrentUsers)
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(new IPEndPoint(IPAddress.Any, portNumber));
            _server.Listen(concurrentUsers);

            _socketClients = new List<Socket>();

            _logger = new Logger();
        }

        public void Start()
        {
            AcceptConnection();
        }

        private static void AcceptConnection()
        {
            _server.BeginAccept(AcceptCallback, null);
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket client = _server.EndAccept(ar);
            _socketClients.Add(client);
            client.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, ReceiveCallback, client);
            AcceptConnection();
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket) ar.AsyncState;
                int size = client.EndReceive(ar);
                byte[] receivedBytes = new byte[size];
                Array.Copy(Buffer, receivedBytes, size);
                string connectionData = Encoding.ASCII.GetString(receivedBytes);

                _logger.Log("User connected! \nConnection details: \n" + connectionData);

                string htmlString = "<div>Index</div>";
                byte[] htmlBytes = Encoding.ASCII.GetBytes(htmlString);
                client.BeginSend(htmlBytes, 0, htmlBytes.Length, SocketFlags.None, SendCallback, client);
            }
            catch (SocketException socketException)
            {
                _logger.Log(socketException.Message);
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            client.Shutdown(SocketShutdown.Send);
            client.Close();
        }
    }
}
