using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    [InitializeOnLoad]
    [ScriptableObjectSingletonAssetPath("", UseAsset = false)]
    public class PersistentContextCacheAsset : ScriptableObjectSingleton<PersistentContextCacheAsset>
    {
        private readonly Dictionary<int, GlobalPersistentContext> _contextCacheByKey =
            new Dictionary<int, GlobalPersistentContext>();
        
        [NonSerialized]
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

        internal bool GetContext<TValue>(int key, out GlobalPersistentContext<TValue> context)
        {
            EnsureInitialize();

            if (_contextCacheByKey.TryGetValue(key, out var originCtx) && originCtx is GlobalPersistentContext<TValue> castedCtx)
            {
                context = castedCtx;
                return false;
            }

            context = GlobalPersistentContext<TValue>.Create();
            _contextCacheByKey[key] = context;
            return true;
        }
    }
}
