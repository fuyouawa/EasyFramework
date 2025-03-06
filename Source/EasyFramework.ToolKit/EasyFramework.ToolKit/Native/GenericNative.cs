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

        public enum ErrorCode
        {
            None = 0,
            SerializerFailed = 1,
            TemplateEngineRenderFailed = 2,
            Unknown = 3
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ErrorCode GetErrorCode();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Buffer GetErrorMsg();

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
    }
}
