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
    public class RectConverter : JsonConverter<Rect>
    {
        public override void WriteJson(JsonWriter writer, Rect value, JsonSerializer serializer)
        {
            // 将 Rect 序列化为 { "x": value.x, "y": value.y, "width": value.width, "height": value.height } 格式
            using (writer.WriteObjectScope())
            {
                writer.WriteProperty("x", value.x);
                writer.WriteProperty("y", value.y);
                writer.WriteProperty("width", value.width);
                writer.WriteProperty("height", value.height);
            }
        }

        public override Rect ReadJson(JsonReader reader, Type objectType, Rect existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Rect(
                obj.GetPropertyOrDefault<float>("x"),
                obj.GetPropertyOrDefault<float>("y"),
                obj.GetPropertyOrDefault<float>("width"),
                obj.GetPropertyOrDefault<float>("height"));
        }
    }
}