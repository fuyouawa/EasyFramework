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
                throw new Exception($"MonoSingleton:\"{typeof(T).Name}\" can only have one instance that exists in the scene");
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
        private static bool s_destroyed;

        public static T Instance
        {
            get
            {
                if (s_destroyed)
                    throw new InvalidOperationException($"Use a singleton:\"{typeof(T)}\" that is already destroyed!");
                if (s_instance == null)
                    s_instance = SingletonCreator.CreateMonoSingleton<T>();
                return s_instance;
            }
        }

        protected virtual void Awake()
        {
            if (s_instance != null)
            {
                if (s_loadBySelf)
                    Destroy(s_instance.gameObject);
                else
                    return;
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
            s_destroyed = true;
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
