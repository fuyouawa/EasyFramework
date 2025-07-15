using EasyToolKit.ThirdParty.OdinSerializer;
using System;

namespace EasyToolKit.Inspector.Editor
{
    public static class InspectorPropertyExtensions
    {
        public static LocalPersistentContext<T> GetPersistentContext<T>(this InspectorProperty property, string key,
            T defaultValue = default)
        {
            var key1 = TwoWaySerializationBinder.Default.BindToName(property.Tree.TargetType);
            var key2 = property.Info.PropertyPath;

            var hash = HashCode.Combine(key1, key2, key);
            return PersistentContext.GetLocal(hash, defaultValue);
        }
    }
}
