using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace EasyGameFramework
{
    public class AnimationCurveConverter : JsonConverter<AnimationCurve>
    {
        public override void WriteJson(JsonWriter writer, AnimationCurve value, JsonSerializer serializer)
        {
            using (writer.WriteArrayScope())
            {
                foreach (var key in value.keys)
                {
                    serializer.Serialize(writer, key);
                }
            }
        }

        public override AnimationCurve ReadJson(JsonReader reader, Type objectType, AnimationCurve existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var ja = JArray.Load(reader);

            var res = new AnimationCurve();
            foreach (var token in ja)
            {
                res.AddKey(token.ToObject<Keyframe>(serializer));
            }
            return res;
        }
    }
}