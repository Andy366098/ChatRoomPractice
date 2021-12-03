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
        private readonly Dictionary<string, IPEndPoint> client = new Dictionary<string, IPEndPoint>();
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
                    
                }
            }
        }
        private void HandleReceiveMessages()
        {

        }
    }
}
