using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class OtherViewBinderConfigsDrawer : OdinValueDrawer<OtherViewBinderConfigs>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Property.Children[0].Draw(EditorHelper.TempContent("配置列表"));
        }
    }
    public class OtherViewBinderConfigDrawer : OdinValueDrawer<OtherViewBinderConfig>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Property.Children[0].Draw(EditorHelper.TempContent("绑定对象"));
            Property.Children[1].Draw(GUIContent.none);
        }
    }
}
