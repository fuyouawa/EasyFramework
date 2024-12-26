using System;
using Newtonsoft.Json;

namespace EasyGameFramework
{
    public class JsonWriteArrayScope : IDisposable
    {
        private readonly JsonWriter _writer;

        public JsonWriteArrayScope(JsonWriter writer)
        {
            _writer = writer;
            _writer.WriteStartArray();
        }


        public void Dispose()
        {
            _writer.WriteEndArray();
        }
    }
}