using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace EasyFramework
{
    public interface ISingleton : IDisposable
    {
        bool IsInitialized { get; }
        void OnSingletonInit();
    }

    internal partial class SingletonCreator
    {
        public static readonly List<ISingleton> Singletons = new List<ISingleton>();

        public static T CreateSingleton<T>() where T : class, ISingleton
        {
            var type = typeof(T);

            if (type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length != 0)
                throw new Exception($"The singleton({type}) cannot have a public constructor!");

            var ctorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

            var ctor = Array.Find(ctorInfos, c => c.GetParameters().Length == 0)
                       ?? throw new Exception(
                           $"The singleton({type}) must have a nonpublic, parameterless constructor!");

            var inst = ctor.Invoke(null) as T;
            if (inst == null)
            {
                throw new Exception($"Instance({type}) construct failed!");
            }

            inst.OnSingletonInit();
            Singletons.Add(inst);

            return inst;
        }
    }

    public class Singleton<T> : ISingleton
        where T : Singleton<T>
    {
        private static readonly Lazy<T> s_instance;

        public static T Instance => s_instance.Value;

        public bool IsSingletonInitialized { get; protected set; }
        bool ISingleton.IsInitialized => IsSingletonInitialized;

        static Singleton()
        {
            s_instance = new Lazy<T>(SingletonCreator.CreateSingleton<T>);
        }

        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
            IsSingletonInitialized = true;
        }

        void IDisposable.Dispose()
        {
            OnSingletonDispose();
            IsSingletonInitialized = false;
        }

        protected virtual void OnSingletonInit()
        {
        }

        protected virtual void OnSingletonDispose()
        {
        }
    }
}
