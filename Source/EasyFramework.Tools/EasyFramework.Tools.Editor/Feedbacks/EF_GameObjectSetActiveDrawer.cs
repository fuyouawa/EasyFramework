using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_GameObjectSetActiveDrawer : AbstractEasyFeedbackDrawer<EF_GameObjectSetActive>
    {
        private InspectorProperty _target;
        private InspectorProperty _activeToSet;

        protected override void Initialize()
        {
            base.Initialize();
            _target = Property.Children["Target"];
            _activeToSet = Property.Children["ActiveToSet"];
        }

        protected override void DrawOtherPropertyLayout()
        {
            _target.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_target, this),
                "游戏对象的激活设置", _target.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    _target.Draw(new GUIContent("目标游戏对象"));
                    _activeToSet.Draw(new GUIContent("激活设置"));
                }
            });
        }
    }
}
