using System;

namespace EasyFramework.Serialization
{
    public interface IEasySerializer
    {
        bool CanSerialize(Type valueType);
    }

    public interface IEasySerializer<T> : IEasySerializer
    {
        void Process(IArchive archive, string name, ref T value);
    }

    public abstract class EasySerializerBase<T> : IEasySerializer<T>
    {
        bool IEasySerializer.CanSerialize(Type valueType)
        {
            return CanSerialize(valueType);
        }

        void IEasySerializer<T>.Process(IArchive archive, string name, ref T value)
        {
            Process(archive, name, ref value);
        }

        protected virtual bool CanSerialize(Type valueType) => true;

        protected virtual void Process(IArchive archive, string name, ref T value)
        {
            if (string.IsNullOrEmpty(name))
            {
                archive.SetNextName(name);
            }
            Process(archive, ref value);
        }

        protected abstract void Process(IArchive archive, ref T value);
    }
}
