using System;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class InspectorPropertyResolver
    {
        public InspectorProperty Property { get; private set; }
        
        public static InspectorPropertyResolver Create([NotNull] Type resolverType, [NotNull] InspectorProperty property)
        {
            if (resolverType == null) throw new ArgumentNullException(nameof(resolverType));
            if (property == null) throw new ArgumentNullException(nameof(property));

            if (!typeof(InspectorPropertyResolver).IsAssignableFrom(resolverType))
            {
                throw new ArgumentException(nameof(resolverType));
            }

            var resolver = resolverType.CreateInstance<InspectorPropertyResolver>();
            resolver.Property = property;
            resolver.Initialize();
            return resolver;
        }

        protected virtual void Initialize() {}

        public abstract InspectorPropertyInfo GetChildInfo(int childIndex);
        public abstract int ChildNameToIndex(string name);
        public abstract int GetChildCount();
    }
}
