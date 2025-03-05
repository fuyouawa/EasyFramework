using System.Runtime.InteropServices;
using System;

namespace EasyFramework.ToolKit
{
    internal static class TemplateEngineNative
    {
        internal const string DllName = "EasyFramework.Core.dll";

        [StructLayout(LayoutKind.Sequential)]
        public struct TemplateEngineEnvironment
        {
            public IntPtr Ptr;
        };

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TemplateEngineEnvironment AllocTemplateEngineEnvironment();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeTemplateEngineEnvironment(TemplateEngineEnvironment environment);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RenderTemplateToStream(GenericNative.IoStream stream, TemplateEngineEnvironment environment, string templateText, string jsonData);
    }
}
