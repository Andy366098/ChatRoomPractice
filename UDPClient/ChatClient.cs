using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDPClient
{
    internal class ChatClient
    {
        private string userName = "";
        private UdpClient client = null;
        private IPEndPoint serverEndPoint = null;

        public ChatClient()
        {

        }
        public bool Connect(string address, int port)
        {
            client = new UdpClient();
            try
            {
                var ipAddress = IPAddress.Parse(address);
                serverEndPoint = new IPEndPoint(ipAddress, port);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine($"發生錯誤 {e}");
                return false;
            }
        }
        public void SetName(string name)
        {
            userName = name;
            var request = "LOGIN:" + userName;
            var buffer = Encoding.UTF8.GetBytes(request);

            client.Send(buffer, buffer.Length, serverEndPoint);
        }
        public void SendMessage(string sMessage)
        {
            var request = "MESSAGE:" + sMessage;
            var buffer = Encoding.UTF8.GetBytes(request);
            client.Send(buffer, buffer.Length, serverEndPoint);
        }
        public void Refresh()
        {
            if(client.Available > 0)
            {
                HandleReceiveMessage();
            }
        }
        private void HandleReceiveMessage()
        {
            IPEndPoint ep = null;
            var buffer = client.Receive(ref ep);
            var request = Encoding.UTF8.GetString(buffer);

            if(request.StartsWith("MESSAGE:", StringComparison.OrdinalIgnoreCase))
            {
                var tokens = request.Split(':');
                var name = tokens[1];
                var message = tokens[2];
                Console.WriteLine($"{name} : {message}");
            }
        }
    }
}
