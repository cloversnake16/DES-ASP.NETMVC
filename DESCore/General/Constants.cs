using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESCore.General
{
    public class Constants
    {
        public const byte SYN = 0x16;
        public const byte DLE = 0x10;
        public const byte BEL = 0x07;
        public const byte ESC = 0x1b;
        public const byte EOT = 0x04;
        public const byte ACK = 0x06;

        public const byte HEART_BEAT = 0xfe;

        public const byte VERSION = (byte)'v';
        public const byte EVENT = (byte)'e';
        public const byte RETREIVE = (byte)'p';
        public const byte PROGRAM = (byte)'P';
        public const byte DOOR = (byte)'D';

        public const byte CHANNEL = (byte)'C';
        public const byte CHANNEL_DOOR = (byte)'c';
        public const byte CHANNEL_TAG = (byte)'G';
        public const byte SCHEDULE = (byte)'S';
        public const byte TRADECODE = (byte)'T';
        public const byte STAFFACCESS = (byte)'A';
        public const byte SYSTEMOPTIONS = (byte)'O';
        public const byte DATETIME = (byte)'K';
        public const byte EVENT_ENABLE = (byte)'E';

        public const string SysAdmin = "SysAdmin";
    }
}
