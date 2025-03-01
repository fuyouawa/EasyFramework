using System.Runtime.InteropServices;
using System;

namespace EasyFramework.Serialization
{
    public partial class EasySerialize
    {
        internal class Native
        {
            public struct IoStream
            {
                public IntPtr ptr;
            }

            public struct OutputArchive
            {
                public IntPtr ptr;
            }

            public struct InputArchive
            {
                public IntPtr ptr;
            }

            public struct Buffer
            {
                public IntPtr ptr;
                public uint size;
            }

            // Buffer Allocation and Freeing
            [DllImport("EasyFramework.Core.dll")]
            public static extern Buffer AllocBuffer(uint size);

            [DllImport("EasyFramework.Core.dll")]
            public static extern void FreeBuffer(Buffer buffer);

            // IoStream Allocation and Freeing
            [DllImport("EasyFramework.Core.dll")]
            public static extern IoStream AllocStringIoStream();

            [DllImport("EasyFramework.Core.dll")]
            public static extern void FreeIoStream(IoStream stream);

            [DllImport("EasyFramework.Core.dll")]
            public static extern Buffer GetIoStreamBuffer(IoStream stream);

            // OutputArchive Allocation and Freeing
            [DllImport("EasyFramework.Core.dll")]
            public static extern OutputArchive AllocBinaryOutputArchive(IoStream stream);

            [DllImport("EasyFramework.Core.dll")]
            public static extern void FreeOutputArchive(OutputArchive archive);

            // InputArchive Allocation and Freeing
            [DllImport("EasyFramework.Core.dll")]
            public static extern InputArchive AllocBinaryInputArchive(IoStream stream);

            [DllImport("EasyFramework.Core.dll")]
            public static extern void FreeInputArchive(InputArchive archive);

            // Write and Read int64
            [DllImport("EasyFramework.Core.dll")]
            public static extern void WriteInt64ToOutputArchive(OutputArchive archive, string name, long value);

            [DllImport("EasyFramework.Core.dll")]
            public static extern long ReadInt64FromInputArchive(InputArchive archive, string name);

            // Write and Read int32
            [DllImport("EasyFramework.Core.dll")]
            public static extern void WriteInt32ToOutputArchive(OutputArchive archive, string name, int value);

            [DllImport("EasyFramework.Core.dll")]
            public static extern int ReadInt32FromInputArchive(InputArchive archive, string name);

            // Write and Read int16
            [DllImport("EasyFramework.Core.dll")]
            public static extern void WriteInt16ToOutputArchive(OutputArchive archive, string name, short value);

            [DllImport("EasyFramework.Core.dll")]
            public static extern short ReadInt16FromInputArchive(InputArchive archive, string name);

            // Write and Read int8
            [DllImport("EasyFramework.Core.dll")]
            public static extern void WriteInt8ToOutputArchive(OutputArchive archive, string name, sbyte value);

            [DllImport("EasyFramework.Core.dll")]
            public static extern sbyte ReadInt8FromInputArchive(InputArchive archive, string name);

            // Write and Read uint64
            [DllImport("EasyFramework.Core.dll")]
            public static extern void WriteUInt64ToOutputArchive(OutputArchive archive, string name, ulong value);

            [DllImport("EasyFramework.Core.dll")]
            public static extern ulong ReadUInt64FromInputArchive(InputArchive archive, string name);

            // Write and Read uint32
            [DllImport("EasyFramework.Core.dll")]
            public static extern void WriteUInt32ToOutputArchive(OutputArchive archive, string name, uint value);

            [DllImport("EasyFramework.Core.dll")]
            public static extern uint ReadUInt32FromInputArchive(InputArchive archive, string name);

            // Write and Read uint16
            [DllImport("EasyFramework.Core.dll")]
            public static extern void WriteUInt16ToOutputArchive(OutputArchive archive, string name, ushort value);

            [DllImport("EasyFramework.Core.dll")]
            public static extern ushort ReadUInt16FromInputArchive(InputArchive archive, string name);

            // Write and Read uint8
            [DllImport("EasyFramework.Core.dll")]
            public static extern void WriteUInt8ToOutputArchive(OutputArchive archive, string name, byte value);

            [DllImport("EasyFramework.Core.dll")]
            public static extern byte ReadUInt8FromInputArchive(InputArchive archive, string name);

            // Write and Read varint32
            [DllImport("EasyFramework.Core.dll")]
            public static extern void WriteVarint32ToOutputArchive(OutputArchive archive, string name, uint value);

            [DllImport("EasyFramework.Core.dll")]
            public static extern uint ReadVarint32FromInputArchive(InputArchive archive, string name);

            // Write and Read float
            [DllImport("EasyFramework.Core.dll")]
            public static extern void WriteFloatToOutputArchive(OutputArchive archive, string name, float value);

            [DllImport("EasyFramework.Core.dll")]
            public static extern float ReadFloatFromInputArchive(InputArchive archive, string name);

            // Write and Read double
            [DllImport("EasyFramework.Core.dll")]
            public static extern void WriteDoubleToOutputArchive(OutputArchive archive, string name, double value);

            [DllImport("EasyFramework.Core.dll")]
            public static extern double ReadDoubleFromInputArchive(InputArchive archive, string name);

            // Write and Read Binary Buffers
            [DllImport("EasyFramework.Core.dll")]
            public static extern void WriteBinaryToOutputArchive(OutputArchive archive, string name, Buffer buffer);

            [DllImport("EasyFramework.Core.dll")]
            public static extern Buffer ReadBinaryFromInputArchive(InputArchive archive, string name);

            // Write and Read Strings
            [DllImport("EasyFramework.Core.dll")]
            public static extern void WriteStringToOutputArchive(OutputArchive archive, string name, string str);

            [DllImport("EasyFramework.Core.dll")]
            public static extern Buffer ReadStringFromInputArchive(InputArchive archive, string name);
        }
    }
}
