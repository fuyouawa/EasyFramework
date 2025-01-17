using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Utilities.Editor
{
    public class NoLabelVector2Drawer : OdinValueDrawer<NoLabelVector2>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Rect contentRect = SirenixEditorGUI.BeginHorizontalPropertyLayout(label, out Rect labelRect);
            EditorGUI.BeginChangeCheck();
            Vector4 val = SirenixEditorFields.VectorPrefixSlideRect(labelRect, ValueEntry.SmartValue.ToVec2());
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = new NoLabelVector2(val.x, val.y);
            }
            GUIHelper.PushLabelWidth(SirenixEditorFields.SingleLetterStructLabelWidth);
            Property.Children[0].Draw(null);
            Property.Children[1].Draw(null);
            GUIHelper.PopLabelWidth();
            SirenixEditorGUI.EndHorizontalPropertyLayout();
        }
    }
}
