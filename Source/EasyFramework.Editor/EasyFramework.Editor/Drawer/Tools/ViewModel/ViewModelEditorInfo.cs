using System;

namespace EasyFramework.Editor.Drawer
{
    [Serializable]
    public class ViewModelEditorInfo
    {
        public string GenerateDir;
        public string Namespace;
        public bool ClassNameSameAsGameObjectName = true;
        public string ClassName;
        public SerializedType BaseClass = new SerializedType();
        public bool IsInitialized;

        public Type GetClassType()
        {
            return ReflectionUtility.FindType(Namespace, ClassName);
        }
    }
}
