using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace EasyGameFramework
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            // 将 Vector2 序列化为 [x, y] 格式
            using (writer.WriteArrayScope())
            {
                writer.WriteValue(value.x);
                writer.WriteValue(value.y);
            }
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            // 从 [x, y] 格式反序列化为 Vector2
            var array = JArray.Load(reader);
            var x = array[0].ToObject<float>();
            var y = array[1].ToObject<float>();
            return new Vector2(x, y);
        }
    }
}
