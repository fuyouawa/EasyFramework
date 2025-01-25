using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;

namespace EasyFramework.Editor
{
    public static class OdinEditorExtension
    {
        public static LocalPersistentContext<TValue> GetPersistent<TValue>(this OdinEditor editor, string key, TValue defaultValue)
        {
            return LocalPersistentContext<TValue>.Create(PersistentContext.Get(
                TwoWaySerializationBinder.Default.BindToName(editor.GetType()),
                TwoWaySerializationBinder.Default.BindToName(editor.Tree.TargetType),
                editor.Tree.RootProperty.Path,
                key,
                defaultValue));
        }
    }
}
