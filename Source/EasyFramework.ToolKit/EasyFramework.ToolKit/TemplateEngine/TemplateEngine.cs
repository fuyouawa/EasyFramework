using System;
using System.Text;
using EasyFramework.Serialization;

namespace EasyFramework.ToolKit.TemplateEngine
{
    public class TemplateEngine : IDisposable
    {
        private readonly TemplateEngineNative.TemplateEngineEnvironment _environment;

        private static readonly EasySerializeSettings SerializeSettings = new EasySerializeSettings()
        {
            MembersGetterOfGeneric = MembersGetterPresets.AllPublicGettable
        };

        public TemplateEngine()
        {
            _environment = TemplateEngineNative.AllocTemplateEngineEnvironment();
        }

        public string Render<T>(string template, T data)
        {
            var jsonData = new EasySerializationData(EasyDataFormat.Json);
            EasySerialize.To(data, ref jsonData, SerializeSettings);
            if (string.IsNullOrEmpty(jsonData.StringData))
                return null;
            var ios = GenericNative.AllocStringIoStream();
            TemplateEngineNative.RenderTemplateToStream(ios, _environment, template, jsonData.StringData);
            var cBuf = GenericNative.GetIoStreamBuffer(ios);
            var buf = GenericNative.ConvertBufferToBytesWithFree(cBuf);
            return Encoding.UTF8.GetString(buf);
        }

        public void Dispose()
        {
            TemplateEngineNative.FreeTemplateEngineEnvironment(_environment);
        }
    }
}
