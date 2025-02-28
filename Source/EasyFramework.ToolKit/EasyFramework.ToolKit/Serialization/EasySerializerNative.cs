using System.Runtime.InteropServices;
using System;

namespace EasyFramework.ToolKit
{
    public class EasySerializerNative
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct OutputStream
        {
            public IntPtr ptr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct InputStream
        {
            public IntPtr ptr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OutputArchive
        {
            public IntPtr ptr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct InputArchive
        {
            public IntPtr ptr;
        }

        [DllImport("EasyFramework.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern OutputStream AllocStringOutputStream();

        [DllImport("EasyFramework.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeOutputStream(OutputStream stream);

        [DllImport("EasyFramework.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern InputStream AllocStringInputStream();

        [DllImport("EasyFramework.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeInputStream(InputStream stream);

        [DllImport("EasyFramework.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong GetInputStreamSize(InputStream stream);

        [DllImport("EasyFramework.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReadInputStream(InputStream stream, IntPtr buffer, ulong bufferSize);

        [DllImport("EasyFramework.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern OutputArchive AllocBinaryOutputArchive(OutputStream stream);

        [DllImport("EasyFramework.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeOutputArchive(OutputArchive archive);

        [DllImport("EasyFramework.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern InputArchive AllocBinaryInputArchive(InputStream stream);

        [DllImport("EasyFramework.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeInputArchive(InputArchive archive);

        [DllImport("EasyFramework.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteToOutputArchive(OutputArchive archive, int value);

        [DllImport("EasyFramework.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReadFromInputArchive(InputArchive archive, ref int value);
    }
}
