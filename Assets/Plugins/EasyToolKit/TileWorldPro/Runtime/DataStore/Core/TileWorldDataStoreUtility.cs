using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyToolKit.Core;

namespace EasyToolKit.TileWorldPro
{
    public static class TileWorldDataStoreUtility
    {
        private static readonly Dictionary<string, Type> DataStoreTypesByName;
        private static string[] s_dataStoreNamesCache;

        static TileWorldDataStoreUtility()
        {
            DataStoreTypesByName = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetCustomAttributes<RegisterTileWorldDataStoreAttribute>())
                .ToDictionary(attribute => attribute.Name, attribute => attribute.DataStoreType);
        }

        public static ITileWorldDataStore GetDefaultDataStore()
        {
            return new BuiltInTileWorldDataStore();
        }

        public static string[] GetDataStoreNamesCache()
        {
            if (s_dataStoreNamesCache == null)
            {
                s_dataStoreNamesCache = DataStoreTypesByName.Keys.ToArray();
            }
            return s_dataStoreNamesCache;
        }

        public static string GetDataStoreNameByType(Type dataStoreType)
        {
            if (!dataStoreType.IsInheritsFrom<ITileWorldDataStore>())
            {
                throw new ArgumentException($"DataStoreType '{dataStoreType}' must implement '{typeof(ITileWorldDataStore)}'", nameof(dataStoreType));
            }

            var dataStoreTypePair = DataStoreTypesByName.FirstOrDefault(pair => pair.Value == dataStoreType);
            if (dataStoreTypePair.Value == null)
            {
                throw new ArgumentException($"DataStoreType '{dataStoreType}' must be registered by defining '{typeof(RegisterTileWorldDataStoreAttribute)}'", nameof(dataStoreType));
            }

            return dataStoreTypePair.Key;
        }

        public static Type GetDataStoreTypeByName(string name)
        {
            if (!DataStoreTypesByName.TryGetValue(name, out var dataStoreType))
            {
                throw new ArgumentException($"DataStore type with name '{name}' not found");
            }
            return dataStoreType;
        }
    }
}