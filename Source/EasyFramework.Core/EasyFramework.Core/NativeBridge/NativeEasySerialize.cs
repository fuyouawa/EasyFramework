// using System.Runtime.InteropServices;
// using System;
//
// namespace EasyFramework.Core.Internal
// {
//     [StructLayout(LayoutKind.Sequential)]
//     public struct NativeOutputArchive
//     {
//         public IntPtr Ptr;
//     }
//
//     [StructLayout(LayoutKind.Sequential)]
//     public struct NativeInputArchive
//     {
//         public IntPtr Ptr;
//     }
//
//     public class NativeOutputArchiveWrapper : IDisposable
//     {
//         public NativeOutputArchive Archive;
//
//         public NativeOutputArchiveWrapper(NativeOutputArchive archive)
//         {
//             Archive = archive;
//         }
//
//         public void Dispose() => NativeEasySerialize.FreeOutputArchiveSafety(Archive);
//     }
//
//     public class NativeInputArchiveWrapper : IDisposable
//     {
//         public NativeInputArchive Archive;
//
//         public NativeInputArchiveWrapper(NativeInputArchive archive)
//         {
//             Archive = archive;
//         }
//
//         public void Dispose() => NativeEasySerialize.FreeInputArchiveSafety(Archive);
//     }
//
//     public static class NativeEasySerialize
//     {
//         /* Archive */
//
//         public static NativeOutputArchive AllocBinaryOutputArchiveSafety(NativeIoStream stream)
//         {
//             var ret = AllocBinaryOutputArchive(stream);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static NativeInputArchive AllocBinaryInputArchiveSafety(NativeIoStream stream)
//         {
//             var ret = AllocBinaryInputArchive(stream);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static NativeOutputArchive AllocJsonOutputArchiveSafety(NativeIoStream stream)
//         {
//             var ret = AllocJsonOutputArchive(stream);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static NativeInputArchive AllocJsonInputArchiveSafety(NativeIoStream stream)
//         {
//             var ret = AllocJsonInputArchive(stream);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static void FreeOutputArchiveSafety(NativeOutputArchive archive)
//         {
//             FreeOutputArchive(archive);
//             NativeUtility.HandleError();
//         }
//
//         public static void FreeInputArchiveSafety(NativeInputArchive archive)
//         {
//             FreeInputArchive(archive);
//             NativeUtility.HandleError();
//         }
//
//         public static void OutputArchiveSetNextNameSafety(NativeOutputArchive archive, string name)
//         {
//             OutputArchiveSetNextName(archive, name);
//             NativeUtility.HandleError();
//         }
//
//         public static void InputArchiveSetNextNameSafety(NativeInputArchive archive, string name)
//         {
//             InputArchiveSetNextName(archive, name);
//             NativeUtility.HandleError();
//         }
//
//         public static void OutputArchiveStartNodeSafety(NativeOutputArchive archive)
//         {
//             OutputArchiveStartNode(archive);
//             NativeUtility.HandleError();
//         }
//
//         public static void OutputArchiveFinishNodeSafety(NativeOutputArchive archive)
//         {
//             OutputArchiveFinishNode(archive);
//             NativeUtility.HandleError();
//         }
//
//         public static void InputArchiveStartNodeSafety(NativeInputArchive archive)
//         {
//             InputArchiveStartNode(archive);
//             NativeUtility.HandleError();
//         }
//
//         public static void InputArchiveFinishNodeSafety(NativeInputArchive archive)
//         {
//             InputArchiveFinishNode(archive);
//             NativeUtility.HandleError();
//         }
//
//         /* Read/Write Signed */
//
//         public static void WriteSizeToOutputArchiveSafety(NativeOutputArchive archive, uint size)
//         {
//             WriteSizeToOutputArchive(archive, size);
//             NativeUtility.HandleError();
//         }
//
//         public static uint ReadSizeFromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadSizeFromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static void WriteInt64ToOutputArchiveSafety(NativeOutputArchive archive, long value)
//         {
//             WriteInt64ToOutputArchive(archive, value);
//             NativeUtility.HandleError();
//         }
//
//         public static long ReadInt64FromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadInt64FromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static void WriteInt32ToOutputArchiveSafety(NativeOutputArchive archive, int value)
//         {
//             WriteInt32ToOutputArchive(archive, value);
//             NativeUtility.HandleError();
//         }
//
//         public static int ReadInt32FromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadInt32FromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static void WriteInt16ToOutputArchiveSafety(NativeOutputArchive archive, short value)
//         {
//             WriteInt16ToOutputArchive(archive, value);
//             NativeUtility.HandleError();
//         }
//
//         public static short ReadInt16FromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadInt16FromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static void WriteInt8ToOutputArchiveSafety(NativeOutputArchive archive, char value)
//         {
//             WriteInt8ToOutputArchive(archive, value);
//             NativeUtility.HandleError();
//         }
//
//         public static char ReadInt8FromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadInt8FromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         /* Read/Write Unsigned */
//
//         public static void WriteUInt64ToOutputArchiveSafety(NativeOutputArchive archive, ulong value)
//         {
//             WriteUInt64ToOutputArchive(archive, value);
//             NativeUtility.HandleError();
//         }
//
//         public static ulong ReadUInt64FromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadUInt64FromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static void WriteUInt32ToOutputArchiveSafety(NativeOutputArchive archive, uint value)
//         {
//             WriteUInt32ToOutputArchive(archive, value);
//             NativeUtility.HandleError();
//         }
//
//         public static uint ReadUInt32FromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadUInt32FromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static void WriteUInt16ToOutputArchiveSafety(NativeOutputArchive archive, ushort value)
//         {
//             WriteUInt16ToOutputArchive(archive, value);
//             NativeUtility.HandleError();
//         }
//
//         public static ushort ReadUInt16FromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadUInt16FromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static void WriteUInt8ToOutputArchiveSafety(NativeOutputArchive archive, byte value)
//         {
//             WriteUInt8ToOutputArchive(archive, value);
//             NativeUtility.HandleError();
//         }
//
//         public static byte ReadUInt8FromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadUInt8FromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         /* Read/Write Boolean */
//
//         public static void WriteBoolToOutputArchiveSafety(NativeOutputArchive archive, byte value)
//         {
//             WriteBoolToOutputArchive(archive, value);
//             NativeUtility.HandleError();
//         }
//
//         public static byte ReadBoolFromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadBoolFromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         /* Read/Write Varint32 */
//
//         public static void WriteVarint32ToOutputArchiveSafety(NativeOutputArchive archive, uint value)
//         {
//             WriteVarint32ToOutputArchive(archive, value);
//             NativeUtility.HandleError();
//         }
//
//         public static uint ReadVarint32FromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadVarint32FromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         /* Read/Write String */
//
//         public static void WriteStringToOutputArchiveSafety(NativeOutputArchive archive, string str)
//         {
//             WriteStringToOutputArchive(archive, str);
//             NativeUtility.HandleError();
//         }
//
//         public static NativeBuffer ReadStringFromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadStringFromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         /* Read/Write Binary */
//
//         public static void WriteBinaryToOutputArchiveSafety(NativeOutputArchive archive, NativeBuffer buffer)
//         {
//             WriteBinaryToOutputArchive(archive, buffer);
//             NativeUtility.HandleError();
//         }
//
//         public static NativeBuffer ReadBinaryFromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadBinaryFromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         /* Read/Write Floating Point */
//
//         public static void WriteFloatToOutputArchiveSafety(NativeOutputArchive archive, float value)
//         {
//             WriteFloatToOutputArchive(archive, value);
//             NativeUtility.HandleError();
//         }
//
//         public static float ReadFloatFromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadFloatFromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static void WriteDoubleToOutputArchiveSafety(NativeOutputArchive archive, double value)
//         {
//             WriteDoubleToOutputArchive(archive, value);
//             NativeUtility.HandleError();
//         }
//
//         public static double ReadDoubleFromInputArchiveSafety(NativeInputArchive archive)
//         {
//             var ret = ReadDoubleFromInputArchive(archive);
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//
//         #region Unsafety
//         
//         internal const string DllName = "EasyFramework.Native";
//
//         /* Archive */
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern NativeOutputArchive AllocBinaryOutputArchive(NativeIoStream stream);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern NativeInputArchive AllocBinaryInputArchive(NativeIoStream stream);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern NativeOutputArchive AllocJsonOutputArchive(NativeIoStream stream);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern NativeInputArchive AllocJsonInputArchive(NativeIoStream stream);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void FreeOutputArchive(NativeOutputArchive archive);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void FreeInputArchive(NativeInputArchive archive);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void OutputArchiveSetNextName(NativeOutputArchive archive, string name);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void InputArchiveSetNextName(NativeInputArchive archive, string name);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void OutputArchiveStartNode(NativeOutputArchive archive);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void OutputArchiveFinishNode(NativeOutputArchive archive);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void InputArchiveStartNode(NativeInputArchive archive);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void InputArchiveFinishNode(NativeInputArchive archive);
//
//         /* Read/Write Signed */
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteSizeToOutputArchive(NativeOutputArchive archive, uint size);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern uint ReadSizeFromInputArchive(NativeInputArchive archive);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteInt64ToOutputArchive(NativeOutputArchive archive, long value);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern long ReadInt64FromInputArchive(NativeInputArchive archive);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteInt32ToOutputArchive(NativeOutputArchive archive, int value);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern int ReadInt32FromInputArchive(NativeInputArchive archive);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteInt16ToOutputArchive(NativeOutputArchive archive, short value);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern short ReadInt16FromInputArchive(NativeInputArchive archive);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteInt8ToOutputArchive(NativeOutputArchive archive, char value);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern char ReadInt8FromInputArchive(NativeInputArchive archive);
//
//         /* Read/Write Unsigned */
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteUInt64ToOutputArchive(NativeOutputArchive archive, ulong value);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern ulong ReadUInt64FromInputArchive(NativeInputArchive archive);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteUInt32ToOutputArchive(NativeOutputArchive archive, uint value);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern uint ReadUInt32FromInputArchive(NativeInputArchive archive);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteUInt16ToOutputArchive(NativeOutputArchive archive, ushort value);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern ushort ReadUInt16FromInputArchive(NativeInputArchive archive);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteUInt8ToOutputArchive(NativeOutputArchive archive, byte value);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern byte ReadUInt8FromInputArchive(NativeInputArchive archive);
//
//         /* Read/Write Boolean */
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteBoolToOutputArchive(NativeOutputArchive archive, byte value);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern byte ReadBoolFromInputArchive(NativeInputArchive archive);
//
//         /* Read/Write Varint32 */
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteVarint32ToOutputArchive(NativeOutputArchive archive, uint value);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern uint ReadVarint32FromInputArchive(NativeInputArchive archive);
//
//         /* Read/Write String */
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteStringToOutputArchive(NativeOutputArchive archive, string str);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern NativeBuffer ReadStringFromInputArchive(NativeInputArchive archive);
//
//         /* Read/Write Binary */
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteBinaryToOutputArchive(NativeOutputArchive archive, NativeBuffer buffer);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern NativeBuffer ReadBinaryFromInputArchive(NativeInputArchive archive);
//
//         /* Read/Write Floating Point */
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteFloatToOutputArchive(NativeOutputArchive archive, float value);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern float ReadFloatFromInputArchive(NativeInputArchive archive);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void WriteDoubleToOutputArchive(NativeOutputArchive archive, double value);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern double ReadDoubleFromInputArchive(NativeInputArchive archive);
//
//         #endregion
//
//         public static NativeInputArchiveWrapper GetWrapper(this NativeInputArchive archive)
//         {
//             return new NativeInputArchiveWrapper(archive);
//         }
//
//         public static NativeOutputArchiveWrapper GetWrapper(this NativeOutputArchive archive)
//         {
//             return new NativeOutputArchiveWrapper(archive);
//         }
//     }
// }
