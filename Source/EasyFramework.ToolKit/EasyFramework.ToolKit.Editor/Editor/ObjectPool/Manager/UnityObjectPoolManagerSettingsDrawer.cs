using System;
using System.Linq;
using EasyFramework.Core;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class UnityObjectPoolManagerSettingsDrawer : OdinValueDrawer<UnityObjectPoolManagerSettings>
    {
        private InspectorProperty _poolTypeProperty;

        private static Type[] _poolTypes;

        private static Type[] PoolTypes => _poolTypes ??= ReflectionUtility.GetAssemblyTypes()
            .Where(type => type.HasInterface(typeof(IUnityObjectPool)) && !type.IsAbstract && !type.IsInterface &&
                           !type.IsGenericType)
            .ToArray();

        protected override void Initialize()
        {
            _poolTypeProperty = Property.Children["_poolType"];
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            EasyEditorGUI.DrawSelectorDropdown(
                () => PoolTypes,
                EditorHelper.TempContent("对象池类型"),
                _poolTypeProperty.GetSmartContent(),
                value => _poolTypeProperty.ValueEntry.SetAllWeakValues(value));
        }
    }
}
