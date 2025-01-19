using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_AuxiliaryRunSecondDrawer : AbstractEasyFeedbackDrawer<EF_AudioSource>
    {
        private InspectorProperty _second;

        protected override void Initialize()
        {
            base.Initialize();
            _second = Property.Children["Second"];
        }

        protected override void DrawOtherPropertyLayout()
        {
            _second.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_second, this),
                "运行指定帧数", _second.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    _second.Draw(new GUIContent("帧数"));
                }
            });
        }
    }
}
