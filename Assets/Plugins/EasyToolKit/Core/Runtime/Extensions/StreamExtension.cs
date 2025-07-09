using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EasyToolKit.Core
{
    public static class StreamExtension
    {
        public static async Task ReadCountAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            var curCount = count;
            while (curCount > 0)
            {
                var read = await stream.ReadAsync(buffer, offset, curCount);
                if (read == 0)
                    throw new EndOfStreamException();
                curCount -= read;
            }
        }
    }
}
