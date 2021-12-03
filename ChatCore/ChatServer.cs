using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatCore
{
    public class ChatServer
    {
        private TcpListener tcpListener;
        private Thread handleThread;
        private readonly Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
        private readonly Dictionary<string, string> userName = new Dictionary<string, string>();
        private readonly Dictionary<string, string> account = new Dictionary<string, string>();
        public ChatServer(){
            //account.Add("A","1234");
            //account.Add("B", "1111");
        }
        public void Bind(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);    //bind一個Port
            Console.WriteLine($"伺服器 start at port {port}");
            tcpListener.Start();   //開始Listen
        }
        public void Start()
        {
            handleThread = new Thread(ClientHandler);
            handleThread.Start();

            try
            {
                while (true)
                {
                    Console.WriteLine("等待連線...");
                    var client = tcpListener.AcceptTcpClient();    //Accept連線

                    var address = client.Client.RemoteEndPoint.ToString();
                    Console.WriteLine($"客戶端已由{address}連線");
                    lock (clients)
                    {
                        clients.Add(address,client);
                    }
                }
                //client.Close();
                //Console.WriteLine($"斷開客戶端{address}的連結");
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                tcpListener.Stop();
                Console.WriteLine("伺服器中止");
            }
        }
        private void ClientHandler()
        {
            while (true)
            {
                var disconnectedClients = new List<string>();   //控管斷線的客戶端List
                lock (clients)//互斥鎖，避免資料同步問題
                {
                    foreach (var address in clients.Keys)
                    {
                        var client = clients[address];
                        try
                        {
                            if (!client.Connected)
                            {
                                disconnectedClients.Add(address);
                            }
                            if (client.Available > 0)
                            {
                                Receive(address);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error {e}");
                        }
                    }
                    foreach(var address in disconnectedClients)
                    {
                        RemoveClient(address);
                    }
                }
            }
        }
        private void RemoveClient(string address)
        {
            Console.WriteLine($"用戶{userName[address]}已斷線");
            var client = clients[address];
            clients.Remove(address);
            userName.Remove(address);
            client.Close();
        }
        private void SendData(TcpClient client,string msg)
        {
            var requestBuffer = System.Text.Encoding.Unicode.GetBytes(msg);   //將訊息轉成位元編碼

            client.GetStream().Write(requestBuffer, 0, requestBuffer.Length);   //將指定的Byte陣列寫入
        }
        private void Receive(string address)
        {
            var client = clients[address];
            var stream = client.GetStream();    //由客戶端獲取的資料流

            var numBytes = client.Available;    //資料流內可供讀取的資料(有幾個Bytes)
            
            var buffer = new byte[numBytes];
            //讀取Byte陣列資料，由0號位置開始讀取numBytes的長度
            var bytesRead = stream.Read(buffer, 0, numBytes);
            //用Unicode解碼方式將讀取進來的byte陣列轉成字串，由指定的位置開始讀某個長度的字串
            var request = System.Text.Encoding.Unicode.GetString(buffer).Substring(0, bytesRead / 2);
            if (request.StartsWith("LOGIN:", StringComparison.OrdinalIgnoreCase))
            {
                var tokens = request.Split(':');

                /*if (tokens.Length != 3)
                {
                    Console.WriteLine("登入格式錯誤，不可包含:字元");
                    SendData(client, "LOGIN:0");
                    return;
                }*/
                var id = tokens[1];
                /*var password = tokens[2];
                if (!account.ContainsKey(id) || account[id] != password)
                {
                    Console.WriteLine("帳號或密碼有誤，無法登入");
                    SendData(client, "LOGIN:0");
                    return;
                }*/
                userName[address] = id;
                Console.WriteLine($"用戶{id}已由{address}登入");
                SendData(client, "LOGIN:1");
                return;
            }
            if (request.StartsWith("MESSAGE:", StringComparison.OrdinalIgnoreCase))
            {
                var tokens = request.Split(':');
                var msg = tokens[1];
                if (!userName.ContainsKey(address))
                {
                    return;
                }
                Console.WriteLine($"{userName[address]} : {msg}");
                Broadcast(address, msg);
            }
                
        }
        public void Broadcast(string address,string message)
        {
            var data = "MESSAGE:" + userName[address] + ":" + message;
            var requestBuffer = System.Text.Encoding.Unicode.GetBytes(data);   //將訊息轉成位元編碼

            foreach(var clientAddress in clients.Keys)
            {
                if (clientAddress != address)
                {
                    try
                    {
                        clients[clientAddress].GetStream().Write(requestBuffer, 0, requestBuffer.Length);   //將指定的Byte陣列寫入

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{userName[clientAddress]}沒接收到訊息，錯誤:{e.Message}");
                    }
                }
            }
        }
    }
}
