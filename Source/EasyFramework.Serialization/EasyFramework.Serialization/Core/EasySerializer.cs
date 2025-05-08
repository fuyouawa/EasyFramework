using System;
using System.Diagnostics;
using EasyFramework.Core;

namespace EasyFramework.Serialization
{
    internal interface IEasySerializer
    {
        bool CanSerialize(Type valueType);
        bool IsRoot { get; set; }
        void Process(ref object value, Type valueType, IArchive archive);
        void Process(string name, ref object value, Type valueType, IArchive archive);
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

        void IEasySerializer.Process(ref object value, Type valueType, IArchive archive)
        {
            ProcessImpl(null, ref value, valueType, archive);
        }

        void IEasySerializer.Process(string name, ref object value, Type valueType, IArchive archive)
        {
            ProcessImpl(name, ref value, valueType, archive);
        }

        private void ProcessImpl(string name, ref object value, Type valueType, IArchive archive)
        {
            Assert.True(typeof(T) == valueType);

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
