using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public sealed class InspectorPropertyInfo
    {
        public SerializedProperty SerializedProperty { get; private set; }

        public static InspectorPropertyInfo CreateForUnityProperty(SerializedProperty serializedProperty)
        {
            var info = new InspectorPropertyInfo();
            info.SerializedProperty = serializedProperty.Copy();
            return info;
        }
    }
}
