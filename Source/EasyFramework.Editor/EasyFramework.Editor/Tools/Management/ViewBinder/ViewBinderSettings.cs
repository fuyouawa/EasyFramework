using Sirenix.OdinInspector;

namespace EasyFramework.Editor
{
    [EditorConfigsAssetPath]
    public class ViewBinderSettings : ScriptableObjectSingleton<ViewBinderSettings>
    {
        [LabelText("自动命名空间")]
        public bool AutoNamingNotations = true;

        [LabelText("注释")]
        public string Comment;

        [LabelText("访问标识符")]
        public ViewBindAccess Access;

        [LabelText("注释自动添加段落xml")]
        public bool AutoAddCommentPara = true;
    }
}
