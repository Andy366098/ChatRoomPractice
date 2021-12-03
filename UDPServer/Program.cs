using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("====================================");
            var server = new ChatServer();
            server.Bind(4099);
            server.Run();
        }
    }
}
