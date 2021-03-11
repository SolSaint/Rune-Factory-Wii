using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Texture_Convert
{
    class BinaryReaderBE : BinaryReader
    {
        public BinaryReaderBE(System.IO.Stream stream) : base(stream) { }

        public int BEReadInt32()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        public Int16 BEReadInt16()
        {
            var data = base.ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public UInt16 BEReadUInt16()
        {
            var data = base.ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToUInt16(data, 0);
        }

        public Int64 BEReadInt64()
        {
            var data = base.ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        public UInt32 BEReadUInt32()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        public float BEReadSingle()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

        public char BEReadChar()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToChar(data, 0);
        }

        public void Skip(uint num)
        {
            base.BaseStream.Seek(num, SeekOrigin.Current);
        }
    }
}
