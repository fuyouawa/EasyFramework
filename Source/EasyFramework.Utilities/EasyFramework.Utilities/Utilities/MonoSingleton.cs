using System;
using EasyFramework.Generic;
using UnityEngine;

namespace EasyFramework.Utilities
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
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = SingletonCreator.CreateMonoSingleton<T>();
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }

            _instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }

        public virtual void OnSingletonInit()
        {
        }

        protected virtual void OnApplicationQuit()
        {
            if (_instance == null) return;
            Destroy(_instance.gameObject);
            _instance = null;
        }

        protected virtual void OnDestroy()
        {
            _instance = null;
        }

        public virtual void OnSingletonInit(SingletonInitialModes mode)
        {
        }
    }
}
