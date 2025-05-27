// using System.Runtime.InteropServices;
// using System;
//
// namespace EasyFramework.Core.Internal
// {
//     public enum NativeErrorCode
//     {
//         None,
//         SerializerFailed,
//         TemplateEngineRenderFailed,
//         BadAlloc,
//         Unknown
//     }
//
//     [StructLayout(LayoutKind.Sequential)]
//     public struct NativeIoStream
//     {
//         public IntPtr Ptr;
//     }
//
//     [StructLayout(LayoutKind.Sequential)]
//     public struct NativeBuffer
//     {
//         public IntPtr Ptr;
//         public uint Size;
//     }
//
//     public class NativeGeneric
//     {
//         public static NativeBuffer AllocBufferSafety(uint size)
//         {
//             var ret = AllocBuffer(size);
//             // NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static void FreeBufferSafety(NativeBuffer buffer)
//         {
//             FreeBuffer(buffer);
//             // NativeUtility.HandleError();
//         }
//
//         public static NativeIoStream AllocStringIoStreamSafety()
//         {
//             var ret = AllocStringIoStream();
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static void FreeIoStreamSafety(NativeIoStream stream)
//         {
//             FreeIoStream(stream);
//             NativeUtility.HandleError();
//         }
//
//         public static void WriteToIoStreamBufferSafety(NativeIoStream stream, NativeBuffer buffer)
//         {
//             WriteToIoStreamBuffer(stream, buffer);
//             NativeUtility.HandleError();
//         }
//
//         public static NativeBuffer GetIoStreamBufferSafety(NativeIoStream stream)
//         {
//             var ret = GetIoStreamBuffer(stream);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         #region Unsafety
//
//         internal const string DllName = "EasyFramework.Native";
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         internal static extern NativeErrorCode GetErrorCode();
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         internal static extern NativeBuffer GetErrorMsg();
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern NativeBuffer AllocBuffer(uint size);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void FreeBuffer(NativeBuffer buffer);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern NativeIoStream AllocStringIoStream();
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void FreeIoStream(NativeIoStream stream);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteToIoStreamBuffer(NativeIoStream stream, NativeBuffer buffer);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern NativeBuffer GetIoStreamBuffer(NativeIoStream stream);
//
//         #endregion
//     }
// }
