using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDPServer
{
    internal class ChatServer
    {
        private UdpClient listener;
        private readonly Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();
        private readonly Dictionary<string, string> userNames = new Dictionary<string, string>();

        public ChatServer()
        {

        }

        public void Bind(int port)
        {
            listener = new UdpClient(port);
            Console.WriteLine($"伺服器開始連線 Port : {port}");

        }
        public void Run()
        {
            while (true)
            {
                var numbytes = listener.Available;
                if(numbytes > 0)
                {
                    HandleReceiveMessages();
                }
            }
        }
        private void HandleReceiveMessages()
        {
            IPEndPoint peerEndPoint = null;
            var buffer = listener.Receive(ref peerEndPoint);
            var request = Encoding.ASCII.GetString(buffer);

            if (request.StartsWith("LOGIN:", StringComparison.OrdinalIgnoreCase))
            {
                var tokens = request.Split(':');
                var clientId = peerEndPoint.ToString();

                clients.Add(clientId, peerEndPoint);
                userNames.Add(clientId, tokens[1]);
                Console.WriteLine($"Client Login : {tokens[1]}");
            }else if(request.StartsWith("MESSAGE:", StringComparison.OrdinalIgnoreCase))
            {
                var tokens = request.Split(':');
                var message = tokens[1];
                var peerId = peerEndPoint.ToString();
                Console.WriteLine($"Receive: {userNames[peerId]}:{tokens[1]}");
                foreach(var clientId in userNames.Keys)
                {
                    if (clientId == peerId)
                    {
                        continue;
                    }
                    var data = "MESSAGE:" + userNames[peerId] + ":" + message;
                    var dataBuffer = Encoding.ASCII.GetBytes(data);

                    listener.Send(dataBuffer, dataBuffer.Length, clients[clientId]);
                }
            }
        }
    }
}
