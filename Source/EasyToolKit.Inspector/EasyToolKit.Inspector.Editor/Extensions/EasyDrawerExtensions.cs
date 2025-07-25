using EasyToolKit.ThirdParty.OdinSerializer;
using System;

namespace EasyToolKit.Inspector.Editor
{
    public static class EasyDrawerExtensions
    {
        public static LocalPersistentContext<T> GetPersistentContext<T>(this IEasyDrawer drawer, string key,
            T defaultValue = default)
        {
            var key1 = TwoWaySerializationBinder.Default.BindToName(drawer.GetType());
            var key2 = TwoWaySerializationBinder.Default.BindToName(drawer.Property.Tree.TargetType);
            var key3 = drawer.Property.Info.PropertyPath;

            return PersistentContext.GetLocal(string.Join("+", key1, key2, key3, key), defaultValue);
        }
    }
}
