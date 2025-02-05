using System;
using System.Collections.Generic;
using System.IO;

namespace EasyFramework
{
    public class CompressionUtility
    {
        public static int DecodeVarint32(Stream stream)
        {
            int result = 0;
            int shift = 0;

            while (true)
            {
                int byteRead = stream.ReadByte();
                if (byteRead == -1)
                    throw new EndOfStreamException("Unexpected end of stream while reading varint.");

                byte b = (byte)byteRead;

                // 将低7位累加到结果中
                result |= (b & 0x7F) << shift;

                // 检查最高位是否为0
                if ((b & 0x80) == 0)
                    break;

                shift += 7;

                if (shift >= 32)
                    throw new FormatException("Malformed Varint32 value.");
            }

            return result;
        }

        public static int DecodeVarint32(byte[] data)
        {
            return DecodeVarint32(data, out _);
        }

        public static int DecodeVarint32(byte[] data, out long position)
        {
            using var stream = new MemoryStream(data);
            var ret = DecodeVarint32(stream);
            position = stream.Position;
            return ret;
        }

        public static byte[] EncodeVarint32(int value)
        {
            var result = new List<byte>();

            while (true)
            {
                // 取低7位
                byte currentByte = (byte)(value & 0x7F);
                value >>= 7;

                // 如果还有更高位，则设置最高位为1
                if (value != 0)
                {
                    currentByte |= 0x80;
                }

                result.Add(currentByte);

                if (value == 0)
                    break;
            }

            return result.ToArray();
        }
    }
}
