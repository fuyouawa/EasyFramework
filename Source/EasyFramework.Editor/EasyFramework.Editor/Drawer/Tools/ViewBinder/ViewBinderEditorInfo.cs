using UnityEngine;

namespace EasyFramework.Editor.Drawer
{
    public enum ViewBindAccess
    {
        Public,
        PrivateWithSerializeFieldAttribute
    }

    public class ViewBinderEditorInfo
    {
        public bool NameSameAsGameObjectName = true;
        public string Name;
        public bool AutoAddCommentPara = true;
        public string Comment;
        public bool IsInitialized;
        public ViewBindAccess Access;
        public bool AutoNamingNotations = true;
        
        public string GetName()
        {
            if (Name.IsNullOrWhiteSpace())
                return Name;
        
            if (!AutoNamingNotations)
                return Name;
        
            //TODO 更多情况的处理
            if (Access == ViewBindAccess.Public)
            {
                return char.ToUpper(Name[0]) + Name[1..];
            }
        
            var name = char.ToLower(Name[0]) + Name[1..];
            return '_' + name;
        }
    }
}
