using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_ParticlePlayDrawer : AbstractEasyFeedbackDrawer<EF_ParticlePlay>
    {
        private InspectorProperty _targetParticleSystem;
        private InspectorProperty _mode;
        private InspectorProperty _emitCount;
        private InspectorProperty _withChildrenParticleSystems;
        private InspectorProperty _randomParticleSystems;
        private InspectorProperty _activateOnPlay;
        private InspectorProperty _stopSystemOnInit;
        private InspectorProperty _duration;

        protected override void Initialize()
        {
            base.Initialize();
            _targetParticleSystem = Property.Children["TargetParticleSystem"];
            _mode = Property.Children["Mode"];
            _emitCount = Property.Children["EmitCount"];
            _withChildrenParticleSystems = Property.Children["WithChildrenParticleSystems"];
            _randomParticleSystems = Property.Children["RandomParticleSystems"];
            _activateOnPlay = Property.Children["ActivateOnPlay"];
            _stopSystemOnInit = Property.Children["StopSystemOnInit"];
            _duration = Property.Children["Duration"];
        }

        protected override void DrawOtherPropertyLayout()
        {
            var value = ValueEntry.SmartValue;

            _targetParticleSystem.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_targetParticleSystem, this),
                "粒子播放", _targetParticleSystem.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    _targetParticleSystem.Draw(new GUIContent("目标粒子系统"));
                    _mode.Draw(new GUIContent("模式"));
                    if (value.Mode == EF_ParticlePlay.Modes.Emit)
                    {
                        _emitCount.Draw(new GUIContent("发射数量"));
                    }
                    _withChildrenParticleSystems.Draw(new GUIContent("包括子粒子系统"));

                    if (value.TargetParticleSystem == null)
                    {
                        _randomParticleSystems.Draw(new GUIContent("随机粒子系统"));
                    }
                    
                    _activateOnPlay.Draw(new GUIContent("播放时激活"));
                    _stopSystemOnInit.Draw(new GUIContent("初始化时停止系统"));
                    _duration.Draw(new GUIContent("持续时间"));
                }
            });
        }    }
}
