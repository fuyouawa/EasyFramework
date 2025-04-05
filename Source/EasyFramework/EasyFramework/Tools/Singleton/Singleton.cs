using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EasyFramework
{
    public interface ISingleton : IDisposable
    {
        void OnSingletonInit();
    }

    internal partial class SingletonCreator
    {
        public static readonly HashSet<ISingleton> Singletons = new HashSet<ISingleton>();

        public static T CreateSingleton<T>() where T : class, ISingleton
        {
            var type = typeof(T);

            ConstructorInfo ctor;
            // 如果基类是抽象类
            if (type.BaseType?.IsAbstract == true)
            {
                // 允许有公开的无参构造函数
                var ctorInfos = type.GetConstructors(BindingFlagsHelper.AllInstance());

                ctor = Array.Find(ctorInfos, c => c.GetParameters().Length == 0);
                if (ctor == null)
                    throw new Exception(
                        $"The singleton of {type} must have a parameterless constructor!");
            }
            // 如果基类不是抽象类
            else
            {
                // 限制不能有公开构造函数
                if (type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length != 0)
                    throw new Exception($"The singleton of {type} cannot have a public constructor!");

                var ctorInfos = type.GetConstructors(BindingFlagsHelper.NonPublicInstance());

                // 限制必须有一个非公开的无参构造函数
                ctor = Array.Find(ctorInfos, c => c.GetParameters().Length == 0);
                if (ctor == null)
                    throw new Exception(
                        $"The singleton of {type} must have a nonpublic, parameterless constructor!");
            }

            var inst = ctor.Invoke(null) as T;
            if (inst == null)
            {
                throw new Exception($"The instance of {type} construct failed!");
            }

            inst.OnSingletonInit();
            var suc = Singletons.Add(inst);
            if (!suc)
            {
                throw new Exception($"The singleton of {type} is not unique!");
            }

            return inst;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterQuitEvent()
        {
            Application.quitting += OnApplicationQuit;
        }

        private static void OnApplicationQuit()
        {
            // foreach (var singleton in Singletons)
            // {
            //     singleton.Dispose();
            // }
            //
            // Singletons.Clear();
        }
    }

    public class Singleton<T> : ISingleton
        where T : Singleton<T>
    {
        private static readonly Lazy<T> s_instance;

        public static T Instance => s_instance.Value;

        static Singleton()
        {
            s_instance = new Lazy<T>(SingletonCreator.CreateSingleton<T>);
        }

        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
        }

        void IDisposable.Dispose()
        {
            OnSingletonDispose();
        }

        protected virtual void OnSingletonInit()
        {
        }

        protected virtual void OnSingletonDispose()
        {
        }
    }
}
