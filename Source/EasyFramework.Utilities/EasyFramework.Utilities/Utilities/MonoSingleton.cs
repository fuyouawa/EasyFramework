using System;
using UnityEngine;

namespace EasyFramework.Utilities
{
    internal partial class SingletonCreator
    {
        public static T CreateMonoSingleton<T>() where T : Component
        {
            if (!Application.isPlaying)
                return null;

            var instances = UnityEngine.Object.FindObjectsOfType<T>();
            if (instances.Length > 1)
            {
                throw new Exception($"Mono单例({typeof(T).Name})在场景中存在的实例只能有1个!");
            }
            if (instances.Length == 1)
            {
                return instances[0];
            }

            var obj = new GameObject(typeof(T).Name);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            var inst = obj.AddComponent<T>();

            return inst;
        }
    }

    public class MonoSingleton<T> : MonoBehaviour
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
    }
}