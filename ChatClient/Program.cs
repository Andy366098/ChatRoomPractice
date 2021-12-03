using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("====================================");
            var client = new ChatCore.ChatClient();
            Console.WriteLine("請輸入你的帳號：");
            var name = Console.ReadLine();
            /*Console.WriteLine("請輸入你的密碼：");
            var password = Console.ReadLine();*/
            var succeedConnect = client.Connect("127.0.0.1",4099);
            if (!succeedConnect)
            {
                return;
            }
            //client.SetName(name,password);
            client.SetName(name);
            while (true)
            {
                while(Console.KeyAvailable == false)
                {
                    client.Refresh();
                    var messages = client.GetMessages();
                    foreach (var message in messages)
                    {
                        Console.WriteLine("{0}: {1}", message.Key, message.Value);
                    }
                    System.Threading.Thread.Sleep(1);
                }
                Console.Write("你 : ");
                var msg = Console.ReadLine();
                if(msg == "exit")
                {
                    Console.WriteLine("88啦") ;
                    client.DisConnect();
                    break;
                }
                client.SendMessage(msg);
            }
        }
        
    }
}
