using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value + 10)]
    public class ReferenceObjectDrawer<T> : EasyValueDrawer<T>
    {
        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return property.Children != null && !property.ValueEntry.ValueType.IsUnityBuiltInType();
        }

        private ReferenceObjectDrawerSettingsAttribute _settings;

        protected override void Initialize()
        {
            _settings = Property.GetAttribute<ReferenceObjectDrawerSettingsAttribute>();
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (label == null)
            {
                CallNextDrawer(null);
                return;
            }

            var containsNull = ValueEntry.Values.Any(value => value == null);

            if (containsNull)
            {
                var instantiateIfNull = InspectorConfigAsset.Instance.TryInstantiateReferenceObjectIfNull || _settings?.InstantiateIfNull == true;

                if (instantiateIfNull)
                {
                    if (ValueEntry.ValueType.IsInstantiable())
                    {
                        InstantiateIfNull();
                        containsNull = false;
                    }
                    else
                    {
                        if (_settings?.InstantiateIfNull == true)
                        {
                            EasyEditorGUI.MessageBox($"Type {ValueEntry.ValueType} is not instiatable.", MessageType.Error);
                            return;
                        }
                    }
                }
            }


            if (containsNull)
            {
                if (_settings?.HideIfNull == true)
                {
                    return;
                }
            }

            if (_settings?.HideFoldout == true || Property.ChildrenResolver is ICollectionResolver)
            {
                CallNextDrawer(label);
                return;
            }

            Property.State.Expanded = EasyEditorGUI.Foldout(Property.State.Expanded, label);

            if (Property.State.Expanded)
            {
                EditorGUI.indentLevel++;
                CallNextDrawer(null);
                EditorGUI.indentLevel--;
            }
        }

        private void InstantiateIfNull()
        {
            for (int i = 0; i < ValueEntry.Values.Count; i++)
            {
                if (ValueEntry.Values[i] == null)
                {
                    ValueEntry.Values[i] = ValueEntry.ValueType.CreateInstance<T>();
                }
            }
        }
    }
}
