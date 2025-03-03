using System.Runtime.InteropServices;
using System;

namespace EasyFramework.Serialization
{
    internal static class EasySerializeNative
    {
        private const string DllName = "EasyFramework.Core.dll";

        [StructLayout(LayoutKind.Sequential)]
        public struct IoStream
        {
            public IntPtr Ptr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OutputArchive
        {
            public IntPtr Ptr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct InputArchive
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

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern OutputArchive AllocBinaryOutputArchive(IoStream stream);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern InputArchive AllocBinaryInputArchive(IoStream stream);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern OutputArchive AllocJsonOutputArchive(IoStream stream);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern InputArchive AllocJsonInputArchive(IoStream stream);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeOutputArchive(OutputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeInputArchive(InputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void OutputArchiveSetNextName(OutputArchive archive, string name);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InputArchiveSetNextName(InputArchive archive, string name);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void OutputArchiveStartNode(OutputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void OutputArchiveFinishNode(OutputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InputArchiveStartNode(InputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InputArchiveFinishNode(InputArchive archive);


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteInt64ToOutputArchive(OutputArchive archive, long value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long ReadInt64FromInputArchive(InputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteInt32ToOutputArchive(OutputArchive archive, int value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReadInt32FromInputArchive(InputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteInt16ToOutputArchive(OutputArchive archive, short value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern short ReadInt16FromInputArchive(InputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteInt8ToOutputArchive(OutputArchive archive, char value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern char ReadInt8FromInputArchive(InputArchive archive);


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteUInt64ToOutputArchive(OutputArchive archive, ulong value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ReadUInt64FromInputArchive(InputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteUInt32ToOutputArchive(OutputArchive archive, uint value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ReadUInt32FromInputArchive(InputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteUInt16ToOutputArchive(OutputArchive archive, ushort value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort ReadUInt16FromInputArchive(InputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteUInt8ToOutputArchive(OutputArchive archive, byte value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ReadUInt8FromInputArchive(InputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteBoolToOutputArchive(OutputArchive archive, byte value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ReadBoolFromInputArchive(InputArchive archive);


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteVarint32ToOutputArchive(OutputArchive archive, uint value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ReadVarint32FromInputArchive(InputArchive archive);


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteStringToOutputArchive(OutputArchive archive, string str);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Buffer ReadStringFromInputArchive(InputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteBinaryToOutputArchive(OutputArchive archive, Buffer buffer);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Buffer ReadBinaryFromInputArchive(InputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteFloatToOutputArchive(OutputArchive archive, float value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern float ReadFloatFromInputArchive(InputArchive archive);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteDoubleToOutputArchive(OutputArchive archive, double value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern double ReadDoubleFromInputArchive(InputArchive archive);


        public class IoStreamWrapper : IDisposable
        {
            public IoStream Stream;

            public IoStreamWrapper(IoStream stream)
            {
                Stream = stream;
            }

            public void Dispose() => FreeIoStream(Stream);
        }

        public class OutputArchiveWrapper : IDisposable
        {
            public OutputArchive Archive;

            public OutputArchiveWrapper(OutputArchive archive)
            {
                Archive = archive;
            }

            public void Dispose() => FreeOutputArchive(Archive);
        }

        public class InputArchiveWrapper : IDisposable
        {
            public InputArchive Archive;

            public InputArchiveWrapper(InputArchive archive)
            {
                Archive = archive;
            }

            public void Dispose() => FreeInputArchive(Archive);
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
