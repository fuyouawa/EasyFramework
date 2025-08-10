using System;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public interface IDrawerChainResolver : IInitializableResolver
    {
        DrawerChain GetDrawerChain();
    }

    public abstract class DrawerChainResolver : IDrawerChainResolver
    {
        public InspectorProperty Property { get; private set; }
        public bool IsInitialized { get; private set; }

        InspectorProperty IInitializableResolver.Property
        {
            get => Property;
            set => Property = value;
        }

        bool IInitializable.IsInitialized => IsInitialized;

        void IInitializable.Initialize()
        {
            if (IsInitialized) return;
            Initialize();
            IsInitialized = true;
        }

        void IInitializable.Deinitialize()
        {
            if (!IsInitialized) return;
            Deinitialize();
            IsInitialized = false;
        }

        protected virtual void Initialize() { }
        protected virtual void Deinitialize() { }

        public abstract DrawerChain GetDrawerChain();
    }
}
