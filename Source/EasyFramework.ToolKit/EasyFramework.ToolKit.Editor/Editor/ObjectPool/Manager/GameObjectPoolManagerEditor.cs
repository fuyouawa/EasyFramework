using System;
using System.Linq;
using EasyFramework.Core;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace EasyFramework.ToolKit.Editor
{
    [CustomEditor(typeof(GameObjectPoolManager))]
    public class GameObjectPoolManagerEditor : OdinEditor
    {
        private InspectorProperty _poolNodeNameProperty;
        private InspectorProperty _poolTypeProperty;
        
        private static Type[] _poolTypes;

        private static Type[] PoolTypes => _poolTypes ??= ReflectionUtility.GetAssemblyTypes()
            .Where(type => type.HasInterface(typeof(IGameObjectPool)) && !type.IsAbstract && !type.IsInterface &&
                           !type.IsGenericType)
            .ToArray();

        protected override void OnEnable()
        {
            base.OnEnable();
            _poolNodeNameProperty = Tree.RootProperty.Children["_poolNodeName"];
            _poolTypeProperty = Tree.RootProperty.Children["_poolType"];
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            EasyEditorGUI.Title("设置");
            _poolNodeNameProperty.DrawEx("对象池节点名称");

            EasyEditorGUI.DrawSelectorDropdown(
                () => PoolTypes,
                EditorHelper.TempContent("对象池类型"),
                _poolTypeProperty.GetSmartContent(),
                value => _poolTypeProperty.ValueEntry.SetAllWeakValues(value));

            Tree.EndDraw();
        }
    }
}
