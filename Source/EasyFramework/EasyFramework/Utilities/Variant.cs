using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyFramework
{
    public enum VariantTypeEnum
    {
        Unsupported,
        Integer,
        Boolean,
        FloatingPoint,
        String,
        UnityObject
    }

    [Serializable]
    public class Variant
    {
        public static bool IsAcceptedType(Type type)
        {
            return type.IsPrimitive || type.IsSubclassOf(typeof(UnityEngine.Object));
        }

        [SerializeField, UsedImplicitly] private int _integralValue;
        [SerializeField, UsedImplicitly] private float _floatingPointValue;
        [SerializeField, UsedImplicitly] private bool _booleanValue;
        [SerializeField, UsedImplicitly] private string _stringValue;
        [SerializeField, UsedImplicitly] private UnityEngine.Object _unityObjectValue;
        [SerializeField, UsedImplicitly] private SerializedType _type;
        [SerializeField] private string _name;

        public Type Type => _type.Value;

        public VariantTypeEnum TypeEnum
        {
            get
            {
                var t = _type.Value;
                if (t.IsIntegerType())
                    return VariantTypeEnum.Integer;
                if (t.IsFloatingPointType())
                    return VariantTypeEnum.FloatingPoint;
                if (t.IsBooleanType())
                    return VariantTypeEnum.Boolean;
                if (t.IsStringType())
                    return VariantTypeEnum.String;
                if (t.IsSubclassOf(typeof(UnityEngine.Object)))
                    return VariantTypeEnum.UnityObject;
                return VariantTypeEnum.Unsupported;
            }
        }

        public string Name => _name;

        public void Setup(Type objectType)
        {
            Setup(objectType, string.Empty);
        }

        public void Setup(Type objectType, string name)
        {
            _type.Value = objectType;
            _name = name;
        }

        public object GetRawValue()
        {
            switch (TypeEnum)
            {
                case VariantTypeEnum.Integer:
                    return _integralValue;
                case VariantTypeEnum.Boolean:
                    return _booleanValue;
                case VariantTypeEnum.FloatingPoint:
                    return _floatingPointValue;
                case VariantTypeEnum.String:
                    return _stringValue;
                case VariantTypeEnum.UnityObject:
                    return _unityObjectValue;
                case VariantTypeEnum.Unsupported:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class ReadOnlyVariantList : List<Variant>
    {
    }
}
