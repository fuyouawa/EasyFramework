using System;
using EasyFramework.Core;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class OtherViewBinderTargetDrawer : OdinValueDrawer<OtherViewBinderTarget>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var val = ValueEntry.SmartValue;
            var comp = GetTargetComponent(Property);

            if (val.Target != null)
            {
                var binders = val.Target.GetComponents(typeof(IViewBinder));
                if (binders.Length == 0)
                {
                    EasyEditorGUI.MessageBox("目标对象必须有绑定器组件！", MessageType.Error);
                }
            }
            else
            {
                EasyEditorGUI.MessageBox("目标对象为空", MessageType.Warning);
            }

            EditorGUI.BeginChangeCheck();
            val.Target = SirenixEditorFields.UnityObjectField(
                EditorHelper.TempContent("目标对象"),
                val.Target, typeof(GameObject), true) as GameObject;

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(comp);
                if (val.Target == null)
                {
                    val.Binder = null;
                }
                else
                {
                    var binders = val.Target.GetComponents(typeof(IViewBinder));
                    val.Binder = binders.Length > 0 ? binders[0] : null;
                }
            }

            if (val.Target != null)
            {
                var binders = val.Target.GetComponents(typeof(IViewBinder));
                if (binders.Length == 0)
                    return;

                if (val.Binder == null)
                {
                    EasyEditorGUI.MessageBox("目标绑定器不能为空！", MessageType.Error);
                }

                GUIContent btnLabel;
                if (val.Binder != null)
                {
                    var idx = Array.IndexOf(binders, val.Binder);
                    if (idx == -1)
                    {
                        val.Binder = null;
                        btnLabel = EditorHelper.NoneSelectorBtnLabel;
                    }
                    else
                    {
                        btnLabel = EditorHelper.TempContent2(
                            $"{idx}.{val.Binder.GetType().GetNiceName()}");
                    }
                }
                else
                {
                    btnLabel = EditorHelper.NoneSelectorBtnLabel;
                }
                
                EditorGUI.BeginChangeCheck();

                EasyEditorGUI.DrawSelectorDropdown(
                    binders,
                    EditorHelper.TempContent("目标绑定器"),
                    btnLabel,
                    value => val.Binder = value,
                    value =>
                    {
                        var idx = Array.IndexOf(binders, value);
                        return $"{idx}.{value.GetType().GetNiceName()}";
                    });

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(comp);
                }
            }
        }

        private static Component GetTargetComponent(InspectorProperty property)
        {
            return property.Parent.Parent.Parent.WeakSmartValue<Component>();
        }
    }
}
