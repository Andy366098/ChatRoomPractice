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
        public virtual void Serialize()
        {
            m_Pos = m_BeginPos;
            _WriteToBuffer(m_Length);
            _WriteToBuffer(m_Command);

        }
        protected bool _WriteToBuffer(int i)
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(i);
            _WriteToBuffer(bytes);
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
