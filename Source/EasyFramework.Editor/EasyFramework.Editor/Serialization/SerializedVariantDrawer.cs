using System;
using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework.Editor
{
    public class SerializedVariantDrawer : OdinValueDrawer<SerializedVariant>
    {
        private SerializedVariant _variant;

        protected override void Initialize()
        {
            base.Initialize();
            _variant = ValueEntry.SmartValue;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (_variant.IsEmpty())
            {
                EditorGUILayout.LabelField(label, EditorHelper.TempContent("Null"));
                return;
            }

            if (_variant.Type.IsIntegerType())
            {
                EditorGUI.BeginChangeCheck();

                var val = _variant.Get<int>();
                EasyEditorField.Value(label, ref val);

                if (EditorGUI.EndChangeCheck())
                {
                    _variant.Set(val);
                }
            }
            else if (_variant.Type.IsFloatingPointType())
            {
                EditorGUI.BeginChangeCheck();

                var val = _variant.Get<float>();
                EasyEditorField.Value(label, ref val);

                if (EditorGUI.EndChangeCheck())
                {
                    _variant.Set(val);
                }
            }
            else if (_variant.Type.IsBooleanType())
            {
                EditorGUI.BeginChangeCheck();

                var val = _variant.Get<bool>();
                EasyEditorField.Value(label, ref val);

                if (EditorGUI.EndChangeCheck())
                {
                    _variant.Set(val);
                }
            }
            else if (_variant.Type.IsStringType())
            {
                EditorGUI.BeginChangeCheck();

                var val = _variant.Get<string>();
                EasyEditorField.Value(label, ref val);

                if (EditorGUI.EndChangeCheck())
                {
                    _variant.Set(val);
                }
            }
            else if (_variant.Type == typeof(Object) || _variant.Type.IsSubclassOf(typeof(Object)))
            {
                EditorGUI.BeginChangeCheck();

                var val = _variant.Get<Object>(true);
                val = SirenixEditorFields.UnityObjectField(label, val, _variant.Type, true);

                if (EditorGUI.EndChangeCheck())
                {
                    _variant.Set(val);
                }
            }
            else
            {
                //TODO 其他类型的支持
                EasyEditorGUI.MessageBox($"The type({_variant.Type}) does not support display!", MessageType.Warning);
            }
        }
    }

    // public class ReadOnlyVariantListDrawer : OdinValueDrawer<ReadOnlyVariantList>
    // {
    //     private FoldoutGroupConfig _foldoutGroupConfig;
    //
    //     protected override void Initialize()
    //     {
    //         base.Initialize();
    //         _foldoutGroupConfig = new FoldoutGroupConfig(
    //             UniqueDrawerKey.Create(Property, this),
    //             GUIContent.none, true,
    //             rect =>
    //             {
    //                 foreach (var child in Property.Children)
    //                 {
    //                     child.Draw();
    //                 }
    //             });
    //     }
    //
    //     protected override void DrawPropertyLayout(GUIContent label)
    //     {
    //         _foldoutGroupConfig.Label = label;
    //         _foldoutGroupConfig.Expand = Property.State.Expanded;
    //
    //         Property.State.Expanded = EasyEditorGUI.FoldoutGroup(_foldoutGroupConfig);
    //     }
    // }
}
