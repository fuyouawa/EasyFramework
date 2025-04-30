using System;
using System.Diagnostics;

namespace EasyFramework.Serialization
{
    internal interface IEasySerializer
    {
        bool CanSerialize(Type valueType);
        bool IsRoot { get; set; }
    }

    internal interface IEasySerializer<T> : IEasySerializer
    {
        void Process(ref T value, IArchive archive);
        void Process(string name, ref T value, IArchive archive);
    }

    public abstract class EasySerializer : IEasySerializer
    {
        public virtual bool CanSerialize(Type valueType) => true;

        protected bool IsRoot { get; private set; }

        bool IEasySerializer.IsRoot
        {
            get => IsRoot;
            set => IsRoot = value;
        }

        protected EasySerializeSettings Settings => EasySerialize.CurrentSettings;

        internal void Process(ref object value, Type valueType, IArchive archive)
        {
            Process(null, ref value, valueType, archive);
        }

        internal abstract void Process(string name, ref object value, Type valueType, IArchive archive);

        public static EasySerializer<T> GetSerializer<T>() => EasySerializersManager.GetSerializer<T>();
    }

    public abstract class EasySerializer<T> : EasySerializer, IEasySerializer<T>
    {
        internal override void Process(string name, ref object value, Type valueType, IArchive archive)
        {
            Debug.Assert(typeof(T) == valueType);

            T val = default;
            if (archive.ArchiveIoType == ArchiveIoTypes.Output)
            {
                val = (T)value;
            }

            Process(name, ref val, archive);

            if (archive.ArchiveIoType == ArchiveIoTypes.Input)
            {
                value = val;
            }
        }

        public void Process(ref T value, IArchive archive)
        {
            Process(null, ref value, archive);
        }

        public abstract void Process(string name, ref T value, IArchive archive);
    }
}
