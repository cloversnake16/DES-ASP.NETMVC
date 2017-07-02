using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.General;

namespace DESCore.DesCommands
{
    public class DesCommand
    {
        public static byte[] GetCommand(byte cmd, byte[] data = null)
        {
            List<byte> cmdBytes = new List<byte>();

            cmdBytes.Add(Constants.SYN);
            cmdBytes.Add(Constants.DLE);
            cmdBytes.Add(Constants.ESC);
            cmdBytes.AddRange(Encoding.ASCII.GetBytes("000000000000"));
            cmdBytes.Add(cmd);
            if (data != null) cmdBytes.AddRange(data);
            cmdBytes.Add(Constants.EOT);
            ushort checkSum = GetCheckSum(cmdBytes, 1, cmdBytes.Count - 1);
            cmdBytes.Add((byte)(checkSum >> 8));
            cmdBytes.Add((byte)checkSum);

            return cmdBytes.ToArray();
        }

        public static byte[] GetDESCommand(byte cmd, byte[] data = null)
        {
            List<byte> cmdBytes = new List<byte>();

            cmdBytes.Add(Constants.DLE);
            cmdBytes.Add(Constants.ACK);
            cmdBytes.AddRange(Encoding.ASCII.GetBytes("000000000000"));
            cmdBytes.Add(cmd);
            if (data != null) cmdBytes.AddRange(data);
            cmdBytes.Add(Constants.EOT);
            ushort checkSum = GetCheckSum(cmdBytes, 1, cmdBytes.Count - 1);
            cmdBytes.Add((byte)(checkSum >> 8));
            cmdBytes.Add((byte)checkSum);

            return cmdBytes.ToArray();
        }

        public static ushort GetCheckSum(List<byte> listByte, int offset, int count)
        {
            ushort checkSum = 0;
            for (int i = 0; i < count; i++) checkSum += listByte[offset + i];
            return (ushort)(checkSum | 0x8080);
        }
    }
}
