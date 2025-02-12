using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EasyFramework
{
    public static class CompressionUtility
    {
        
        public static async Task<int> ReadVarint32Async(this Stream stream)
        {
            int result = 0;
            int shift = 0;

            while (true)
            {
                var buf = new byte[1];
                int i = await stream.ReadAsync(buf, 0, 1);
                if (i <= 0)
                    throw new EndOfStreamException("Unexpected end of stream while reading varint.");

                byte b = buf[0];

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

        public static int ReadVarint32(this Stream stream)
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

        public static int DecodeVarint32(this byte[] data)
        {
            return DecodeVarint32(data, out _);
        }

        public static int DecodeVarint32(this byte[] data, out long position)
        {
            using var stream = new MemoryStream(data);
            var ret = ReadVarint32(stream);
            position = stream.Position;
            return ret;
        }

        public static byte[] EncodeVarint32(this int value)
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
