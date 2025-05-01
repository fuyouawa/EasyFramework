using System.Runtime.InteropServices;
using System.Text;
using System;

namespace EasyFramework.Core
{
    public static class NativeUtility
    {
        public class IoStreamWrapper : IDisposable
        {
            public NativeIoStream Stream;

            public IoStreamWrapper(NativeIoStream stream)
            {
                Stream = stream;
            }

            public void Dispose() => NativeGeneric.FreeIoStreamSafety(Stream);
        }

        public class BufferWrapper : IDisposable
        {
            public NativeBuffer Buffer;

            public BufferWrapper(NativeBuffer buffer)
            {
                Buffer = buffer;
            }

            public void Dispose() => NativeGeneric.FreeBufferSafety(Buffer);
        }

        public static IoStreamWrapper GetWrapper(this NativeIoStream stream)
        {
            return new IoStreamWrapper(stream);
        }

        public static BufferWrapper GetWrapper(this NativeBuffer buffer)
        {
            return new BufferWrapper(buffer);
        }

        public static byte[] ToBytesWithFree(this NativeBuffer buffer)
        {
            using (new BufferWrapper(buffer))
            {
                var data = new byte[buffer.Size];
                Marshal.Copy(buffer.Ptr, data, 0, data.Length);
                return data;
            }
        }

        public static string ToStringWithFree(this NativeBuffer buffer)
        {
            var cBuf = ToBytesWithFree(buffer);
            return Encoding.UTF8.GetString(cBuf);
        }

        public static NativeBuffer ToNativeBuffer(this byte[] data)
        {
            var buffer = NativeGeneric.AllocBufferSafety((uint)data.Length);
            try
            {
                Marshal.Copy(data, 0, buffer.Ptr, data.Length);
            }
            catch (Exception)
            {
                NativeGeneric.FreeBufferSafety(buffer);
                throw;
            }

            return buffer;
        }

        public static void HandleError()
        {
            var ec = NativeGeneric.GetErrorCode();
            if (ec == NativeErrorCode.None)
                return;
            
            var msgBuf = NativeGeneric.GetErrorMsg();
            var msg = ToStringWithFree(msgBuf);

            throw new NativeException(ec, msg);
        }
    }
}
