using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using EasyFramework.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Utilities.Editor
{
    public class GameObjectReferenceDrawer : OdinValueDrawer<GameObjectReference>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var val = ValueEntry.SmartValue;

            var target = val.DeRef();

            using (new EditorGUILayout.HorizontalScope())
            {
                var obj = SirenixEditorFields.UnityObjectField(label, target, typeof(GameObject), true) as GameObject;
                if (obj != target)
                {
                    ValueEntry.SmartValue = new GameObjectReference(obj);
                }

                if (obj == null)
                {
                    if (val.AbsolutePath.IsNullOrEmpty())
                    {
                        var icon = EditorGUIUtility.IconContent("d_redLight");
                        EditorGUILayout.LabelField(
                            new GUIContent(icon.image, $"无任何引用"),
                            GUILayout.Width(EditorGUIUtility.singleLineHeight));
                    }
                    else
                    {
                        var icon = EditorGUIUtility.IconContent("d_orangeLight");
                        EditorGUILayout.LabelField(
                            new GUIContent(icon.image, $"{val.AbsolutePath}\n对象还未加载"),
                            GUILayout.Width(EditorGUIUtility.singleLineHeight));

                        var rect = EditorGUILayout.GetControlRect(false,
                            GUILayout.Width(EditorGUIUtility.singleLineHeight),
                            GUILayout.Height(EditorGUIUtility.singleLineHeight));

                        if (SirenixEditorGUI.SDFIconButton(
                                rect,
                                new GUIContent("", "重置引用"),
                                SdfIconType.ArrowClockwise))
                        {
                            ValueEntry.SmartValue = new GameObjectReference();
                        }
                    }
                }
                else
                {
                    var icon = EditorGUIUtility.IconContent("d_greenLight");
                    EditorGUILayout.LabelField(
                        new GUIContent(icon.image, $"{val.AbsolutePath}"),
                        GUILayout.Width(EditorGUIUtility.singleLineHeight));
                }
            }
        }
    }
}
