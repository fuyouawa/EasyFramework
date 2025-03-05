using System.Runtime.InteropServices;
using System;

namespace EasyFramework.ToolKit
{
    internal class GenericNative
    {
        internal const string DllName = "EasyFramework.Core.dll";

        [StructLayout(LayoutKind.Sequential)]
        public struct IoStream
        {
            public IntPtr Ptr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Buffer
        {
            public IntPtr Ptr;
            public uint Size;
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Buffer AllocBuffer(uint size);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeBuffer(Buffer buffer);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IoStream AllocStringIoStream();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeIoStream(IoStream stream);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteToIoStreamBuffer(IoStream stream, Buffer buffer);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Buffer GetIoStreamBuffer(IoStream stream);


        public class IoStreamWrapper : IDisposable
        {
            public IoStream Stream;

            public IoStreamWrapper(IoStream stream)
            {
                Stream = stream;
            }

            public void Dispose() => FreeIoStream(Stream);
        }

        public class BufferWrapper : IDisposable
        {
            public Buffer Buffer;

            public BufferWrapper(Buffer buffer)
            {
                Buffer = buffer;
            }

            public void Dispose() => FreeBuffer(Buffer);
        }

        public static byte[] ConvertBufferToBytesWithFree(Buffer buffer)
        {
            using (new BufferWrapper(buffer))
            {
                var data = new byte[buffer.Size];
                Marshal.Copy(buffer.Ptr, data, 0, data.Length);
                return data;
            }
        }

        public static Buffer ConvertBytesToBuffer(byte[] data)
        {
            var buffer = AllocBuffer((uint)data.Length);
            try
            {
                Marshal.Copy(data, 0, buffer.Ptr, data.Length);
            }
            catch (Exception)
            {
                FreeBuffer(buffer);
                throw;
            }

            return buffer;
        }
    }
}
