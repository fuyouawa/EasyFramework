using System;
using System.Diagnostics;
using System.Reflection;

namespace EasyFramework.Generic
{
    internal class SingletonCreator
    {
        public static T CreateSingleton<T>() where T : class
        {
            var type = typeof(T);

            if (type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length != 0)
                throw new Exception($"The singleton({type}) cannot have a public constructor!");

            var ctorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

            var ctor = Array.Find(ctorInfos, c => c.GetParameters().Length == 0)
                       ?? throw new Exception($"The singleton({type}) must have a nonpublic, parameterless constructor!");

            var inst = ctor.Invoke(null) as T;
            Debug.Assert(inst != null);
            return inst;
        }
    }

    public class Singleton<T> where T : Singleton<T>
    {
        private static readonly Lazy<T> _instance;

        public static T Instance => _instance.Value;

        static Singleton()
        {
            _instance = new Lazy<T>(SingletonCreator.CreateSingleton<T>);
        }
    }
}
