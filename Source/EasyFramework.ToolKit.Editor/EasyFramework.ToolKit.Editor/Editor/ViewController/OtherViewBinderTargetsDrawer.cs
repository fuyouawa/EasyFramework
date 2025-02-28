using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class OtherViewBinderTargetsDrawer : OdinValueDrawer<OtherViewBinderTargets>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Property.Children[0].Draw(EditorHelper.TempContent("绑定对象列表"));
        }
    }
}
