using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Core.Editor.Internal;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [InitializeOnLoad]
    [ModuleEditorConfigsPath("Inspector")]
    public class PersistentContextCacheAsset : ScriptableObjectSingleton<PersistentContextCacheAsset>, ISerializationCallbackReceiver
    {
        private int? _totalCacheDataSize = 0;

        [SerializeField] private bool _saveCacheOnUnityQuit = false;

        public int CacheCount => _cache?.Count ?? 0;
        public int TotalCacheDataSize
        {
            get
            {
                if (_totalCacheDataSize == null)
                {
                    CalculateTotalCacheDataSize();
                }
                return _totalCacheDataSize ?? 0;
            }
        }

        [NonSerialized, OdinSerialize]
        private Dictionary<string, GlobalPersistentContext> _cache;

        [SerializeField, HideInInspector]
        private SerializationData _serializationData;

        private bool _initialized = false;

        static PersistentContextCacheAsset()
        {
            UnityEditorEventUtility.DelayAction(() => Instance.EnsureInitialize());
        }

        private void EnsureInitialize()
        {
            if (!_initialized)
            {
                //TODO Initialize PersistentContextCache
                _initialized = true;
            }
        }

        public bool GetContext<TValue>(string key, out GlobalPersistentContext<TValue> context)
        {
            EnsureInitialize();

            if (_saveCacheOnUnityQuit)
            {
                EasyEditorUtility.SetUnityObjectDirty(this);
            }

            if (_cache.TryGetValue(key, out var originCtx) && originCtx is GlobalPersistentContext<TValue> castedCtx)
            {
                context = castedCtx;
                return false;
            }

            context = GlobalPersistentContext<TValue>.Create();
            _cache[key] = context;
            return true;
        }

        public void ClearCache()
        {
            _cache.Clear();
            _totalCacheDataSize = 0;
            EasyEditorUtility.SetUnityObjectDirty(this);
        }

        private void CalculateTotalCacheDataSize()
        {
            _totalCacheDataSize = _serializationData.CalculateNearlyBinarySize();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData);
            _cache ??= new Dictionary<string, GlobalPersistentContext>();
            _totalCacheDataSize = null;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref _serializationData);
            _cache ??= new Dictionary<string, GlobalPersistentContext>();
        }
    }
}
