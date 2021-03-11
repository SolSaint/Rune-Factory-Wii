using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Texture_Convert
{
    class BinaryWriterBE : BinaryWriter
    {
        public BinaryWriterBE(System.IO.Stream stream) : base(stream) { }

        public void BEWrite(int num)
        {
            var data = BitConverter.GetBytes(num);
            Array.Reverse(data);
            base.Write(data);
        }

        public void BEWrite(UInt32 num)
        {
            var data = BitConverter.GetBytes(num);
            Array.Reverse(data);
            base.Write(data);
        }

        public void BEWrite(Int16 num)
        {
            var data = BitConverter.GetBytes(num);
            Array.Reverse(data);
            base.Write(data);
        }

        public void BEWrite(UInt16 num)
        {
            var data = BitConverter.GetBytes(num);
            Array.Reverse(data);
            base.Write(data);
        }

        public void BEWrite(Int64 num)
        {
            var data = BitConverter.GetBytes(num);
            Array.Reverse(data);
            base.Write(data);
        }

        public void BEWrite(UInt64 num)
        {
            var data = BitConverter.GetBytes(num);
            Array.Reverse(data);
            base.Write(data);
        }

        public void BEWrite(float num)
        {
            var data = BitConverter.GetBytes(num);
            Array.Reverse(data);
            base.Write(data);
        }

        public void BEWrite(byte[] data)
        {
            Array.Reverse(data);
            base.Write(data);
        }
    }
}
