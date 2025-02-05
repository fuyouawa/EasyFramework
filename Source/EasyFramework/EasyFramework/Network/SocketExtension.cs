using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System;
using UnityEngine;

namespace Pokemon
{
    public static class SocketExtension
    {
        public static async Task<byte[]> ReceiveAsync(this Socket socket, int size)
        {
            if (size == 0)
                return Array.Empty<byte>();

            var buffer = new byte[size];

            int readSizeTotal = 0;
            while (readSizeTotal < size)
            {
                var curReadSize = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer, readSizeTotal, size - readSizeTotal),
                    SocketFlags.None);
                if (curReadSize == 0)
                    throw new SocketException((int)SocketError.ConnectionReset);
                readSizeTotal += curReadSize;
            }

            return buffer;
        }

        public static async Task<int> ReceiveInt32Async(this Socket socket)
        {
            var intBytes = await socket.ReceiveAsync(sizeof(int));
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(intBytes));
        }
    }
}
