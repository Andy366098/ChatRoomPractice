using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatCore
{
    class Command
    {
        public enum Type
        {
            LOGIN = 1,
            MESSAGE
        }
        private int m_Command;
        public int CommandID => m_Command;

        private int m_Length;

        private byte[] m_PacketBuffer = new byte[1024];
        private int m_BeginPos;
        private int m_Pos;

        protected Command(int command)
        {
            m_Command = command;
        }
        public static void FetchHeader(out int length, out int command, byte[] packetData, int beginPos)
        {
            var header = new Command(0);
            //中間還有兩行沒打
            length = header.m_Length;
            command = header.m_Command;
        }
        public virtual void Serialize() //序列化，這裡應該是分包寫入資料
        {
            m_Pos = m_BeginPos;
            _WriteToBuffer(m_Length);
            _WriteToBuffer(m_Command);
        }
        public virtual void Unserialize()   //解包
        {
            m_Pos = m_BeginPos;
            _ReadFromBuffer(out m_Length);
            _ReadFromBuffer(out m_Command);
        }
        public byte[] SealPacketBuffer(out int iLength) //將包前面多加一個長度
        {
            m_Length = m_Pos;

            var curPos = m_Pos;
            m_Pos = m_BeginPos;
            _WriteToBuffer(m_Length);
            m_Pos = curPos;
            iLength = m_Length;
            return m_PacketBuffer;
        }
        
        protected bool _WriteToBuffer(int i)    //將整數型別資料轉成byte陣列
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(i);
            _WriteToBuffer(bytes);
            return true;
        }
        
        protected bool _ReadFromBuffer(out int i)
        {
            if (BitConverter.IsLittleEndian)
            {
                var byteData = new byte[sizeof(int)];
                Buffer.BlockCopy(m_PacketBuffer, m_BeginPos + m_Pos, byteData, 0, byteData.Length);
                Array.Reverse(byteData);
                i = BitConverter.ToInt32(byteData, 0);
            }
            else
            {
                i = BitConverter.ToInt32(m_PacketBuffer, m_BeginPos + m_Pos);
            }
            m_Pos += sizeof(int);
            return true;
        }
        private void _WriteToBuffer(byte[] byteData)
        {
            // converter little-endian to network's big-endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteData);
            }

            byteData.CopyTo(m_PacketBuffer, m_BeginPos + m_Pos);
            m_Pos += byteData.Length;
        }
    }
}
