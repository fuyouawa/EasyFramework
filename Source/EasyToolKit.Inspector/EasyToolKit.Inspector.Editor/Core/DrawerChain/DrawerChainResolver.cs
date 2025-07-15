using System;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class DrawerChainResolver
    {
        public InspectorProperty Property { get; private set; }

        public static DrawerChainResolver Create([NotNull] Type resolverType, [NotNull] InspectorProperty property)
        {
            if (resolverType == null) throw new ArgumentNullException(nameof(resolverType));
            if (property == null) throw new ArgumentNullException(nameof(property));

            if (!typeof(DrawerChainResolver).IsAssignableFrom(resolverType))
            {
                throw new ArgumentException(nameof(resolverType));
            }

            var resolver = resolverType.CreateInstance<DrawerChainResolver>();
            resolver.Property = property;
            resolver.Initialize();
            return resolver;
        }

        protected virtual void Initialize() {}

        public abstract DrawerChain GetDrawerChain();
    }
}
