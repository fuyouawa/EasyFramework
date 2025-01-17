using System;
using System.Collections.Generic;
using EasyFramework.Generic;
using UnityEngine;

namespace EasyFramework.Utilities
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

        [SerializeField] private int _integralValue;
        [SerializeField] private float _floatingPointValue;
        [SerializeField] private bool _booleanValue;
        [SerializeField] private string _stringValue;
        [SerializeField] private UnityEngine.Object _unityObjectValue;
        [SerializeField] private string _name;
        [SerializeField] private SerializedType _type;

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
