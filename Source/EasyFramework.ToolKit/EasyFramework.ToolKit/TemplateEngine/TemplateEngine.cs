using System;
using EasyFramework.Core.Internal;
using EasyFramework.Serialization;

namespace EasyFramework.ToolKit
{
    public class TemplateEngine : IDisposable
    {
        private readonly NativeTemplateEngineEnvironment _environment;

        private static readonly EasySerializeSettings SerializeSettings = new EasySerializeSettings(MemberFilterPresets.AllPublicGettable);

        public TemplateEngine()
        {
            try
            {
                _environment = NativeTemplateEngine.AllocTemplateEngineEnvironmentSafety();
            }
            catch (NativeException e)
            {
                if (e.ErrorCode == NativeErrorCode.TemplateEngineRenderFailed)
                    throw new TemplateEngineException(e.Message);

                throw;
            }
        }

        public string Render<T>(string template, T data)
        {
            try
            {
                var jsonData = new EasySerializationData(EasyDataFormat.Json);
                EasySerialize.To(data, ref jsonData, SerializeSettings);
                if (string.IsNullOrEmpty(jsonData.StringData))
                    return null;
                var ios = NativeGeneric.AllocStringIoStreamSafety();
                NativeTemplateEngine.RenderTemplateToStreamSafety(ios, _environment, template, jsonData.StringData);
                var cBuf = NativeGeneric.GetIoStreamBufferSafety(ios);
                return cBuf.ToStringWithFree();
            }
            catch (NativeException e)
            {
                if (e.ErrorCode == NativeErrorCode.TemplateEngineRenderFailed)
                    throw new TemplateEngineException(e.Message);

                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                NativeTemplateEngine.FreeTemplateEngineEnvironmentSafety(_environment);
            }
            catch (NativeException e)
            {
                if (e.ErrorCode == NativeErrorCode.TemplateEngineRenderFailed)
                    throw new TemplateEngineException(e.Message);

                throw;
            }
        }
    }
}
