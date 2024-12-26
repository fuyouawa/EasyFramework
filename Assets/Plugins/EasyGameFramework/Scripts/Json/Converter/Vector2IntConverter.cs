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
    public class Vector2IntConverter : JsonConverter<Vector2Int>
    {
        public override void WriteJson(JsonWriter writer, Vector2Int value, JsonSerializer serializer)
        {
            using (writer.WriteArrayScope())
            {
                writer.WriteValue(value.x);
                writer.WriteValue(value.y);
            }
        }

        public override Vector2Int ReadJson(JsonReader reader, Type objectType, Vector2Int existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            var x = array[0].ToObject<int>();
            var y = array[1].ToObject<int>();
            return new Vector2Int(x, y);
        }
    }
}
