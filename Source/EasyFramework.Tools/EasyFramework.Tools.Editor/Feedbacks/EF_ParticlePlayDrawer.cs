using EasyFramework.Generic;
using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_ParticlePlayDrawer : AbstractEasyFeedbackDrawer<EF_ParticlePlay>
    {
        private InspectorProperty _randomParticleSystemsProperty;

        protected override void Initialize()
        {
            base.Initialize();
            _randomParticleSystemsProperty = Property.Children[nameof(EF_ParticlePlay.RandomParticleSystems)];
        }

        protected override void DrawOtherPropertyLayout()
        {
            FreeExpand1 = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                FreeKey1, "粒子系统设置", FreeExpand1)
            {
                OnContentGUI = rect =>
                {
                    if (Feedback.TargetParticleSystem == null && Feedback.RandomParticleSystems.IsNullOrEmpty())
                    {
                        EasyEditorGUI.MessageBox("粒子系统不能为空！", MessageType.Error);
                    }

                    Feedback.TargetParticleSystem = EasyEditorField.UnityObject(
                        EditorHelper.TempContent("粒子系统"),
                        Feedback.TargetParticleSystem);

                    if (Feedback.TargetParticleSystem == null)
                    {
                        _randomParticleSystemsProperty.Draw(EditorHelper.TempContent("随机粒子系统"));
                    }
                    
                    Feedback.Duration = EasyEditorField.Value(
                        EditorHelper.TempContent("持续时间"),
                        Feedback.Duration);

                    Feedback.StopSystemOnInit = EasyEditorField.Value(
                        EditorHelper.TempContent("初始化时停止系统"),
                        Feedback.StopSystemOnInit);

                    Feedback.ActivateOnPlay = EasyEditorField.Value(
                        EditorHelper.TempContent("播放时激活"),
                        Feedback.ActivateOnPlay);
                }
            });
            FreeExpand2 = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                FreeKey2, "播放设置", FreeExpand2)
            {
                OnContentGUI = rect =>
                {
                    Feedback.Mode = EasyEditorField.Enum(
                        EditorHelper.TempContent("模式"),
                        Feedback.Mode);

                    if (Feedback.Mode == EF_ParticlePlay.Modes.Emit)
                    {
                        Feedback.EmitCount = EasyEditorField.Value(
                            EditorHelper.TempContent("发射数量"),
                            Feedback.EmitCount);
                    }

                    Feedback.WithChildrenParticleSystems = EasyEditorField.Value(
                        EditorHelper.TempContent("包括子粒子系统"),
                        Feedback.WithChildrenParticleSystems);
                }
            });
        }    }
}
