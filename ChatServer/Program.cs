using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        static HashSet<TcpClient> clients = new HashSet<TcpClient>();
        static void Main(string[] args)
        {
            Console.WriteLine("=========分隔線==========");
            var server = new ChatCore.ChatServer();
            server.Bind(4099);
            server.Start();
        }
        
        
    }
}
