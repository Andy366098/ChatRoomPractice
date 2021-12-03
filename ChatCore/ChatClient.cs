using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatCore
{
    public class ChatClient
    {
        private TcpClient client;
        private List<KeyValuePair<string, string>> messageList;
        public ChatClient()
        {
            messageList = new List<KeyValuePair<string, string>>();
        }
        public bool Connect(string ip,int port)
        {
            //Console.WriteLine("=======分=========隔========");
            client = new TcpClient();
            try
            {
                Console.WriteLine($"正在連線到聊天伺服器{ip} : {port}");
                client.Connect(ip, port);

                Console.WriteLine("已連接到伺服器");
                return client.Connected;
                
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine($"ArgumentNullException: {e}");
                return false;
            }
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException: {e}");
                return false;
            }
        }
        public void DisConnect()
        {
            client.Close();
            Console.WriteLine("斷開與伺服器的連結");
        }
        public void DoCommand(string cmd)
        {
            int.TryParse(cmd, out int result);
            if (result == 1)
            {
                
            }
        }
        public void SetName(string name)
        {
            var data = "LOGIN:" + name;
            SendData(data);
        }
        public void SetName(string name , string password)
        {
            var data = "LOGIN:" + name + ":" + password;
            SendData(data);
        }
        public void SendMessage(string msg)
        {
            var data = "MESSAGE:" + msg;
            SendData(data);
        }
        private void SendData(string msg)
        {
            var requestBuffer = System.Text.Encoding.Unicode.GetBytes(msg);   //將訊息轉成位元編碼

            client.GetStream().Write(requestBuffer, 0, requestBuffer.Length);   //將指定的Byte陣列寫入
        }
        
        public void Refresh()
        {
            if(client.Available > 0)
            {
                Receive(client);
            }
        }
        public List<KeyValuePair<string, string>> GetMessages()
        {
            var messages = new List<KeyValuePair<string, string>>(messageList);
            messageList.Clear();

            return messages;
        }
        private void Receive(TcpClient client)
        {
            var stream = client.GetStream();    //由客戶端獲取的資料流

            var numBytes = client.Available;    //資料流內可供讀取的資料(有幾個Bytes)
            var buffer = new byte[numBytes];
            //讀取Byte陣列資料，由0號位置開始讀取numBytes的長度
            var bytesRead = stream.Read(buffer, 0, numBytes);
            //用Unicode解碼方式將讀取進來的byte陣列轉成字串，由指定的位置開始讀某個長度的字串
            var request = System.Text.Encoding.Unicode.GetString(buffer).Substring(0, bytesRead / 2);
            if (request.StartsWith("LOGIN:1", StringComparison.OrdinalIgnoreCase))
            {
                //Console.WriteLine("Login succeed");
                messageList.Add(new KeyValuePair<string, string>("System", "Login succeed"));
                return;
            }
            if (request.StartsWith("LOGIN:0", StringComparison.OrdinalIgnoreCase))
            {
                //Console.WriteLine("Login failed");
                messageList.Add(new KeyValuePair<string, string>("System", "Login failed"));
                return;
            }
            if (request.StartsWith("MESSAGE:", StringComparison.OrdinalIgnoreCase))
            {
                var tokens = request.Split(':');
                var sender = tokens[1];
                var msg = tokens[2];
                //Console.WriteLine($"{sender} : {msg}");
                messageList.Add(new KeyValuePair<string, string>(sender, msg));
            }

        }
        /*public string UnityRefresh()
        {
            string msg = "";
            if (client.Available > 0)
            {
               msg = UnityReceive(client);
            }
            return msg;
        }
        private string UnityReceive(TcpClient client)
        {
            var stream = client.GetStream();    //由客戶端獲取的資料流

            var numBytes = client.Available;    //資料流內可供讀取的資料(有幾個Bytes)
            var buffer = new byte[numBytes];
            //讀取Byte陣列資料，由0號位置開始讀取numBytes的長度
            var bytesRead = stream.Read(buffer, 0, numBytes);
            //用Unicode解碼方式將讀取進來的byte陣列轉成字串，由指定的位置開始讀某個長度的字串
            var request = System.Text.Encoding.Unicode.GetString(buffer).Substring(0, bytesRead / 2);
            if (request.StartsWith("LOGIN:1", StringComparison.OrdinalIgnoreCase))
            {
                return "Login succeed";
            }
            if (request.StartsWith("LOGIN:0", StringComparison.OrdinalIgnoreCase))
            {
                return "Login failed";
            }
            if (request.StartsWith("MESSAGE:", StringComparison.OrdinalIgnoreCase))
            {
                var tokens = request.Split(':');
                var sender = tokens[1];
                var msg = tokens[2];
                return $"{sender} : {msg}";
            }
            return "";

        }*/
    }
}
