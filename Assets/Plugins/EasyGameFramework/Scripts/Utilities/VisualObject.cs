using System;
using EasyFramework;
using Sirenix.OdinInspector;
using EasyGameFramework;

namespace EasyGameFramework
{
    [Serializable]
    public class VisualObject
    {
        public static bool IsAcceptedType(Type type)
        {
            return type.IsPrimitive || type.IsSubclassOf(typeof(UnityEngine.Object));
        }

        [LabelText("@Label")]
        [ShowIf("IsInteger")]
        public int IntegralValue;
        [LabelText("@Label")]
        [ShowIf("IsFloat")]
        public float FloatingPointValue;
        [LabelText("@Label")]
        [ShowIf("IsBool")]
        public bool BooleanValue;
        [LabelText("@Label")]
        [ShowIf("IsString")]
        public string StringValue;
        [LabelText("@Label")]
        [ShowIf("IsUnityObject")]
        public UnityEngine.Object UnityObjectValue;

        public string Label { get; private set; }
        public Type ObjectType { get; private set; }

        public void Setup(Type type, string label)
        {
            Label = label ?? GetDefaultLabel();
            ObjectType = type;
        }

        private bool IsInteger => ObjectType.IsIntegerType();
        private bool IsFloat => ObjectType.IsFloatType();
        private bool IsBool => ObjectType.IsBoolType();
        private bool IsString => ObjectType.IsStringType();
        private bool IsUnityObject => ObjectType.IsSubclassOf(typeof(UnityEngine.Object));

        public object GetRawValue()
        {
            if (IsInteger)
                return IntegralValue;
            if (IsFloat)
                return FloatingPointValue;
            if (IsBool)
                return BooleanValue;
            if (IsString)
                return StringValue;
            if (IsUnityObject)
                return UnityObjectValue;
            return null;
        }

        private string GetDefaultLabel()
        {
            var val = GetRawValue();
            return $"`{val.GetType().GetAliases()}`";
        }
    }
}
