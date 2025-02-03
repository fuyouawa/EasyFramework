using System;
using UnityEngine;

namespace EasyFramework
{
    public enum SingletonInitialModes
    {
        Load,
        Create
    }

    public interface IUnitySingleton
    {
        void OnSingletonInit(SingletonInitialModes mode);
    }

    internal partial class SingletonCreator
    {
        public static T CreateMonoSingleton<T>() where T : Component, IUnitySingleton
        {
            if (!Application.isPlaying)
                return null;

            T inst;
            var instances = UnityEngine.Object.FindObjectsOfType<T>();
            if (instances.Length > 1)
            {
                throw new Exception($"Mono单例({typeof(T).Name})在场景中存在的实例只能有1个!");
            }
            if (instances.Length == 1)
            {
                inst = instances[0];
                inst.OnSingletonInit(SingletonInitialModes.Load);
            }
            else
            {
                var obj = new GameObject(typeof(T).Name);
                UnityEngine.Object.DontDestroyOnLoad(obj);
                inst = obj.AddComponent<T>();
                inst.OnSingletonInit(SingletonInitialModes.Create);
            }


            return inst;
        }
    }

    public class MonoSingleton<T> : MonoBehaviour, IUnitySingleton
        where T : MonoSingleton<T>
    {
        private static T s_instance;
        private static bool s_loadBySelf;

        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = SingletonCreator.CreateMonoSingleton<T>();
                }
                return s_instance;
            }
        }

        protected virtual void Awake()
        {
            if (s_instance != null && s_loadBySelf)
            {
                Destroy(s_instance.gameObject);
            }

            s_instance = (T)this;
            s_loadBySelf = true;
            DontDestroyOnLoad(gameObject);
        }

        public virtual void OnSingletonInit()
        {
        }

        protected virtual void OnApplicationQuit()
        {
            if (s_instance == null) return;
            Destroy(s_instance.gameObject);
            s_instance = null;
        }

        protected virtual void OnDestroy()
        {
            s_instance = null;
        }

        void IUnitySingleton.OnSingletonInit(SingletonInitialModes mode)
        {
            OnSingletonInit(mode);
        }

        protected virtual void OnSingletonInit(SingletonInitialModes mode)
        {

        }
    }
}
