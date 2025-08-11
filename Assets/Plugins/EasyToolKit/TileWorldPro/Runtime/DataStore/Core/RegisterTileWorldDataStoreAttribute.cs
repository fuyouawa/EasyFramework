using System;

namespace EasyToolKit.TileWorldPro
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterTileWorldDataStoreAttribute : Attribute
    {
        public Type DataStoreType { get; }
        public string Name { get; }

        public RegisterTileWorldDataStoreAttribute(Type dataStoreType, string name)
        {
            if (!typeof(ITileWorldDataStore).IsAssignableFrom(dataStoreType))
            {
                throw new ArgumentException($"DataStoreType '{dataStoreType}' must implement '{typeof(ITileWorldDataStore)}'", nameof(dataStoreType));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be empty", nameof(name));
            }

            DataStoreType = dataStoreType;
            Name = name;
        }
    }
}