using System.Runtime.InteropServices;
using System.Text;
using System;

namespace EasyFramework.Serialization
{
    internal static class NativeUtility
    {
        public class IoStreamWrapper : IDisposable
        {
            public GenericNative.IoStream Stream;

            public IoStreamWrapper(GenericNative.IoStream stream)
            {
                Stream = stream;
            }

            public void Dispose() => GenericNative.FreeIoStream(Stream);
        }

        public class BufferWrapper : IDisposable
        {
            public GenericNative.Buffer Buffer;

            public BufferWrapper(GenericNative.Buffer buffer)
            {
                Buffer = buffer;
            }

            public void Dispose() => GenericNative.FreeBuffer(Buffer);
        }

        public static IoStreamWrapper GetWrapper(this GenericNative.IoStream stream)
        {
            return new IoStreamWrapper(stream);
        }

        public static BufferWrapper GetWrapper(this GenericNative.Buffer buffer)
        {
            return new BufferWrapper(buffer);
        }

        public static byte[] ToBytesWithFree(this GenericNative.Buffer buffer)
        {
            using (new BufferWrapper(buffer))
            {
                var data = new byte[buffer.Size];
                Marshal.Copy(buffer.Ptr, data, 0, data.Length);
                return data;
            }
        }

        public static string ToStringWithFree(this GenericNative.Buffer buffer)
        {
            var cBuf = ToBytesWithFree(buffer);
            return Encoding.UTF8.GetString(cBuf);
        }

        public static GenericNative.Buffer ToNativeBuffer(this byte[] data)
        {
            var buffer = GenericNative.AllocBuffer((uint)data.Length);
            try
            {
                Marshal.Copy(data, 0, buffer.Ptr, data.Length);
            }
            catch (Exception)
            {
                GenericNative.FreeBuffer(buffer);
                throw;
            }

            return buffer;
        }

        public static void HandleSerializerError()
        {
            var ec = GenericNative.GetErrorCode();
            if (ec == GenericNative.ErrorCode.None)
                return;
            
            var msgBuf = GenericNative.GetErrorMsg();
            var msg = ToStringWithFree(msgBuf);

            switch (ec)
            {
                case GenericNative.ErrorCode.SerializerFailed:
                    throw new EasySerializationException(msg);
                case GenericNative.ErrorCode.Unknown:
                    throw new Exception(msg);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
