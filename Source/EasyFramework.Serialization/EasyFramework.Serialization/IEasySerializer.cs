using System;

namespace EasyFramework.Serialization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EasySerializePriorityAttribute : Attribute
    {
        public int Priority;

        public EasySerializePriorityAttribute()
            : this(0)
        {
        }

        public EasySerializePriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }

    public interface IDataReader
    {
        bool ReadInt32(out int value);
        bool ReadFloat(out float value);
        bool ReadDouble(out double value);
        bool ReadString(out string value);
    }

    public interface IEasySerializer<T>
    {
        bool CanSerialize(Type objectType);

        void Read(ref T value);
        void Write(ref T value);
    }
}
