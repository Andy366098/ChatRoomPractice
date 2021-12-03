using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("================");
            var client = new ChatClient();
            Console.WriteLine("請輸入你的名字");
            var name = Console.ReadLine();

            var succeed = client.Connect("127.0.0.1", 4099);
            if (!succeed)
            {
                return;
            }
            client.SetName(name);
            Console.WriteLine("連線成功，現在可打字輸入");
            while (true)
            {
                while(Console.KeyAvailable == false)
                {
                    client.Refresh();
                    System.Threading.Thread.Sleep(1);
                }
                Console.Write("你 :");
                var msg = Console.ReadLine();

                if(msg == "exit")
                {
                    Console.WriteLine("88拉");
                }
                
                client.SendMessage(msg);
            }
        }
    }
}
