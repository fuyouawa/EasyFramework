using System;

namespace EasyFramework.Serialization
{
    internal interface IEasySerializer
    {
        bool CanSerialize(Type valueType);
        bool IsRoot { get; set; }
        Type ValueType { get; }

        void Process(ref object value, IArchive archive);
        void Process(string name, ref object value, IArchive archive);
    }

    public abstract class EasySerializer<T> : IEasySerializer
    {
        bool IEasySerializer.CanSerialize(Type valueType)
        {
            return CanSerialize(valueType);
        }

        bool IEasySerializer.IsRoot
        {
            get => IsRoot;
            set => IsRoot = value;
        }

        public Type ValueType => typeof(T);

        void IEasySerializer.Process(ref object value, IArchive archive)
        {
            ProcessImpl(null, ref value, archive);
        }

        void IEasySerializer.Process(string name, ref object value, IArchive archive)
        {
            ProcessImpl(name, ref value, archive);
        }

        private void ProcessImpl(string name, ref object value, IArchive archive)
        {
            T val = default;

            var archiveIoType = archive.ArchiveIoType;
            if (archiveIoType == ArchiveIoType.Output)
            {
                val = (T)value;
            }

            Process(name, ref val, archive);

            if (archiveIoType == ArchiveIoType.Input)
            {
                value = val;
            }
        }
        
        protected bool IsRoot { get; private set; }
        
        protected EasySerializeSettings Settings => EasySerialize.CurrentSettings;

        public virtual bool CanSerialize(Type valueType) => true;

        public void Process(ref T value, IArchive archive)
        {
            Process(null, ref value, archive);
        }

        public abstract void Process(string name, ref T value, IArchive archive);

        public static EasySerializer<T> GetSerializer<T>() => EasySerializersManager.Instance.GetSerializer<T>();
    }
}
