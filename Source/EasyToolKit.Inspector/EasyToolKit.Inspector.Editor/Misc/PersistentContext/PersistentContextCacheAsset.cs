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
    [EditorConfigsPath]
    public class PersistentContextCacheAsset : ScriptableObjectSingleton<PersistentContextCacheAsset>, ISerializationCallbackReceiver
    {
        [NonSerialized, OdinSerialize]
        private Dictionary<string, GlobalPersistentContext> _contextCache;

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

        internal bool GetContext<TValue>(string key, out GlobalPersistentContext<TValue> context)
        {
            EnsureInitialize();

            if (_contextCache.TryGetValue(key, out var originCtx) && originCtx is GlobalPersistentContext<TValue> castedCtx)
            {
                context = castedCtx;
                return false;
            }

            context = GlobalPersistentContext<TValue>.Create();
            _contextCache[key] = context;
            return true;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _contextCache ??= new Dictionary<string, GlobalPersistentContext>();
            UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref _serializationData);
            _contextCache ??= new Dictionary<string, GlobalPersistentContext>();
        }
    }
}
