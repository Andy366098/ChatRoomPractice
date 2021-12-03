using System;
using System.Text;

namespace ChatCoreTest
{
    internal class Program
    {
        private static byte[] m_PacketData;
        private static uint m_Pos = 0;
        public enum Type{
            TypeInt,
            TypeFloat,
            TypeString
        }
        public static void Main(string[] args)
        {
            m_PacketData = new byte[1024];
            m_Pos = 0;
            
            Write(109);
            Write(109.99f);
            Write("Hello!");
            


            Console.Write($"Output Byte array(length:{m_Pos}): ");
            for (var i = 0; i < m_Pos; i++)
            {
                Console.Write(m_PacketData[i] + ", ");
            }
            Console.WriteLine("");
            Console.WriteLine($"input Byte array(length:{m_Pos}): ");
            Read(m_PacketData);
            
            
        }

        // write an integer into a byte array
        private static bool Write(int i)
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(i);
            var type = BitConverter.GetBytes((int)Type.TypeInt);
            
            _Write(type);
            _Write(bytes);
            return true;
        }
        private static bool Write2(int i)   //不存type的，方便寫字串時寫入長度
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(i);
            
            _Write(bytes);
            return true;
        }
        // write a float into a byte array
        private static bool Write(float f)
        {
            // convert int to byte array
            var type = BitConverter.GetBytes((int)Type.TypeFloat);
            var bytes = BitConverter.GetBytes(f);
            _Write(type);
            _Write(bytes);
            return true;
        }

        // write a string into a byte array
        private static bool Write(string s)
        {
            // convert string to byte array 
            var type = BitConverter.GetBytes((int)Type.TypeString);
            var bytes = Encoding.Unicode.GetBytes(s);
            _Write(type);
            // write byte array length to packet's byte array
            if (Write2(bytes.Length) == false)
            {
                return false;
            }

            _Write(bytes);
            return true;
        }

        // write a byte array into packet's byte array
        private static void _Write(byte[] byteData)
        {
            // converter little-endian to network's big-endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteData);
            }
            
            byteData.CopyTo(m_PacketData, m_Pos);
            m_Pos += (uint)byteData.Length;
            
        }
        private static bool Read(byte[] packetData)
        {
            Array.Reverse(packetData, 0,(int) m_Pos); 
            int length = 0;
            
            
            for (int i = (int)m_Pos - 4; i > 0; i = i - 8 - length)
            {
                int type = BitConverter.ToInt32(packetData, i);
                
                switch (type)
                {
                    case 0:
                        length = 0;
                        int intData = BitConverter.ToInt32(packetData, i - 4);
                        Console.WriteLine(intData) ;
                        break;
                    case 1:
                        length = 0;
                        float floatData = BitConverter.ToSingle(packetData, i - 4);
                        Console.WriteLine(floatData);
                        break;
                    case 2:
                        length = BitConverter.ToInt32(packetData, i - 4);
                        string data = Encoding.Unicode.GetString(packetData, i - 4 - length, length);
                        Console.WriteLine(data);
                        break;
                }
            }
            return true;
        }
        
    }
}
