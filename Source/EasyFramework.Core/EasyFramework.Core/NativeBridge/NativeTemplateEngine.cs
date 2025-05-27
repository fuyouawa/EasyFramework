// using System.Runtime.InteropServices;
// using System;
//
// namespace EasyFramework.Core.Internal
// {
//     [StructLayout(LayoutKind.Sequential)]
//     public struct NativeTemplateEngineEnvironment
//     {
//         public IntPtr Ptr;
//     };
//
//     public static class NativeTemplateEngine
//     {
//         public static NativeTemplateEngineEnvironment AllocTemplateEngineEnvironmentSafety()
//         {
//             var ret = AllocTemplateEngineEnvironment();
//             NativeUtility.HandleError();
//             return ret;
//         }
//
//         public static void FreeTemplateEngineEnvironmentSafety(NativeTemplateEngineEnvironment environment)
//         {
//             FreeTemplateEngineEnvironment(environment);
//             NativeUtility.HandleError();
//         }
//
//         public static void RenderTemplateToStreamSafety(NativeIoStream stream,
//             NativeTemplateEngineEnvironment environment, string templateText, string jsonData)
//         {
//             RenderTemplateToStream(stream, environment, templateText, jsonData);
//             NativeUtility.HandleError();
//         }
//
//
//         #region Unsafety
//
//         internal const string DllName = "EasyFramework.Native";
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern NativeTemplateEngineEnvironment AllocTemplateEngineEnvironment();
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void FreeTemplateEngineEnvironment(NativeTemplateEngineEnvironment environment);
//
//         [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
//         private static extern void RenderTemplateToStream(NativeIoStream stream,
//             NativeTemplateEngineEnvironment environment, string templateText, string jsonData);
//
//         #endregion
//     }
// }
