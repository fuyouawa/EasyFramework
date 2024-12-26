using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace EasyGameFramework
{
    public class KeyframeConverter : JsonConverter<Keyframe>
    {
        public override void WriteJson(JsonWriter writer, Keyframe value, JsonSerializer serializer)
        {
            using (writer.WriteObjectScope())
            {
                writer.WriteProperty("value", value.value);
                writer.WriteProperty("inTangent", value.inTangent);
                writer.WriteProperty("inWeight", value.inWeight);
                writer.WriteProperty("outTangent", value.outTangent);
                writer.WriteProperty("outWeight", value.outWeight);
                writer.WriteProperty("time", value.time);
                writer.WriteProperty("weightedMode", value.weightedMode);
            }
        }

        public override Keyframe ReadJson(JsonReader reader, Type objectType, Keyframe existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var res = new Keyframe()
            {
                value = jo.GetPropertyOrDefault<float>("value"),
                inTangent = jo.GetPropertyOrDefault<float>("inTangent"),
                inWeight = jo.GetPropertyOrDefault<float>("inWeight"),
                outTangent = jo.GetPropertyOrDefault<float>("outTangent"),
                outWeight = jo.GetPropertyOrDefault<float>("outWeight"),
                time = jo.GetPropertyOrDefault<float>("time"),
                weightedMode = jo.GetPropertyOrDefault<WeightedMode>("weightedMode")
            };
            return res;
        }
    }
}