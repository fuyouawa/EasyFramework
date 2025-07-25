namespace EasyToolKit.Inspector.Editor
{
    public static class PersistentContext
    {
        public static GlobalPersistentContext<TValue> Get<TValue>(string key, TValue defaultValue)
        {
            if (PersistentContextCacheAsset.Instance.GetContext<TValue>(key, out var context))
            {
                context.Value = defaultValue;
            }

            return context;
        }

        public static bool Get<TValue>(string key, out GlobalPersistentContext<TValue> context)
        {
            return PersistentContextCacheAsset.Instance.GetContext<TValue>(key, out context);
        }

        public static LocalPersistentContext<TValue> GetLocal<TValue>(string key, TValue defaultValue)
        {
            return LocalPersistentContext<TValue>.Create(Get(key, defaultValue));
        }

        public static bool GetLocal<TValue>(string key, out LocalPersistentContext<TValue> context)
        {
            var isNew = PersistentContextCacheAsset.Instance.GetContext<TValue>(key, out var globalCtx);
            context = LocalPersistentContext<TValue>.Create(globalCtx);
            return isNew;
        }
    }
}
