using System;
using EasyFramework.Generic;
using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework.Utilities.Editor
{
    [DrawerPriority(0, 0, 2001)]
    public class VariantDrawer : OdinValueDrawer<Variant>
    {
        private InspectorProperty _integralValue;
        private InspectorProperty _floatingPointValue;
        private InspectorProperty _booleanValue;
        private InspectorProperty _stringValue;
        private InspectorProperty _unityObjectValue;

        private Object UnityObjectValue
        {
            get => _unityObjectValue.ValueEntry.WeakSmartValue as Object;
            set => _unityObjectValue.ValueEntry.WeakSmartValue = value;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _integralValue = Property.Children["_integralValue"];
            _floatingPointValue = Property.Children["_floatingPointValue"];
            _booleanValue = Property.Children["_booleanValue"];
            _stringValue = Property.Children["_stringValue"];
            _unityObjectValue = Property.Children["_unityObjectValue"];
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            var lbl = value.Name.IsNotNullOrEmpty() ? new GUIContent(value.Name) : label;

            switch (value.TypeEnum)
            {
                case VariantTypeEnum.Integer:
                    _integralValue.Draw(lbl);
                    break;
                case VariantTypeEnum.Boolean:
                    _booleanValue.Draw(lbl);
                    break;
                case VariantTypeEnum.FloatingPoint:
                    _floatingPointValue.Draw(lbl);
                    break;
                case VariantTypeEnum.String:
                    _stringValue.Draw(lbl);
                    break;
                case VariantTypeEnum.UnityObject:
                {
                    UnityObjectValue = SirenixEditorFields.UnityObjectField(lbl, UnityObjectValue, value.Type, true);
                    break;
                }
                case VariantTypeEnum.Unsupported:
                    EasyEditorGUI.MessageBox($"Unsupported Type!", MessageType.Error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class ReadOnlyVariantListDrawer : OdinValueDrawer<ReadOnlyVariantList>
    {
        private FoldoutGroupConfig _foldoutGroupConfig;

        protected override void Initialize()
        {
            base.Initialize();
            _foldoutGroupConfig = new FoldoutGroupConfig(
                UniqueDrawerKey.Create(Property, this),
                GUIContent.none, true,
                rect =>
                {
                    foreach (var child in Property.Children)
                    {
                        child.Draw();
                    }
                });
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            _foldoutGroupConfig.Label = label;
            _foldoutGroupConfig.Expand = Property.State.Expanded;

            Property.State.Expanded = EasyEditorGUI.FoldoutGroup(_foldoutGroupConfig);
        }
    }
}
