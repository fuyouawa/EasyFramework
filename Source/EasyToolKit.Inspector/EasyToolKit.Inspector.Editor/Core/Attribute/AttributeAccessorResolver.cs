using System;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class AttributeAccessorResolver
    {
        public InspectorProperty Property { get; private set; }

        public static AttributeAccessorResolver Create([NotNull] Type resolverType, [NotNull] InspectorProperty property)
        {
            if (resolverType == null) throw new ArgumentNullException(nameof(resolverType));
            if (property == null) throw new ArgumentNullException(nameof(property));

            if (!typeof(AttributeAccessorResolver).IsAssignableFrom(resolverType))
            {
                throw new ArgumentException(nameof(resolverType));
            }

            var resolver = resolverType.CreateInstance<AttributeAccessorResolver>();
            resolver.Property = property;
            resolver.Initialize();
            return resolver;
        }

        protected virtual void Initialize() {}

        public abstract IAttributeAccessor[] GetAttributeAccessors();
    }
}
