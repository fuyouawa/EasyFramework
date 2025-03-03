using System;

namespace EasyFramework.Serialization
{
    public interface IEasySerializer
    {
        bool CanSerialize(Type valueType);
    }

    public interface IEasySerializer<T> : IEasySerializer
    {
        void Process(IArchive archive, ref T value);
    }

    public abstract class EasySerializer<T> : IEasySerializer<T>
    {
        bool IEasySerializer.CanSerialize(Type valueType)
        {
            return CanSerialize(valueType);
        }

        void IEasySerializer<T>.Process(IArchive archive, ref T value)
        {
            Process(archive, ref value);
        }

        protected virtual bool CanSerialize(Type valueType) => true;
        protected abstract void Process(IArchive archive, ref T value);
    }
}
