using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyToolKit.Core;

namespace EasyToolKit.TileWorldPro
{
    public static class TileBuildProcessorUtility
    {
        private static readonly Dictionary<string, Type> ProcessorTypesByName;
        private static string[] s_processorNamesCache;

        static TileBuildProcessorUtility()
        {
            ProcessorTypesByName = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetCustomAttributes<RegisterTileBuildProcessorAttribute>())
                .ToDictionary(attribute => attribute.Name, attribute => attribute.ProcessorType);
        }

        public static string[] GetProcessorNamesCache()
        {
            if (s_processorNamesCache == null)
            {
                s_processorNamesCache = ProcessorTypesByName.Keys.ToArray();
            }
            return s_processorNamesCache;
        }

        public static string GetProcessorNameByType(Type processorType)
        {
            if (!processorType.IsInheritsFrom<ITileBuildProcessor>())
            {
                throw new ArgumentException($"Processor type '{processorType}' must implement '{typeof(ITileBuildProcessor)}'", nameof(processorType));
            }

            var processorTypePair = ProcessorTypesByName.FirstOrDefault(pair => pair.Value == processorType);
            if (processorTypePair.Value == null)
            {
                throw new ArgumentException($"Processor type '{processorType}' must be registered by defining '{typeof(RegisterTileBuildProcessorAttribute)}'", nameof(processorType));
            }

            return processorTypePair.Key;
        }

        public static Type GetProcessorTypeByName(string name)
        {
            if (!ProcessorTypesByName.TryGetValue(name, out var processorType))
            {
                throw new ArgumentException($"Processor type with name '{name}' not found");
            }
            return processorType;
        }
    }
}