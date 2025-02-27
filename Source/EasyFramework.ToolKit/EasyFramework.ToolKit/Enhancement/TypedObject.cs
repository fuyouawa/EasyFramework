using System;

namespace EasyFramework.ToolKit
{
    public struct TypedObject
    {
        public object Value { get; }
        public Type ObjectType { get; }


        public TypedObject(object value, Type objectType)
        {
            Value = value;
            ObjectType = objectType;
        }
    }
}
