using System;
using EasyFramework.Core;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EasyFramework.Modules
{
    [AddFeedbackMenu("粒子/播放粒子")]
    public class ParticlePlayFeedback : AbstractFeedback
    {
        public enum Modes
        {
            Play,
            Stop,
            Pause,
            Emit
        }

        [FoldoutGroup("粒子系统设置")]
        [InfoBoxEx("粒子系统不能为空！", InfoMessageType.Error, nameof(ShowTargetError))]
        [LabelText("粒子系统")]
        public ParticleSystem Target;

        [FoldoutGroup("粒子系统设置")]
        [ShowIf(nameof(ShowRandomTargets))]
        [LabelText("随机粒子系统")]
        public ParticleSystem[] RandomTargets = Array.Empty<ParticleSystem>();
        
        [FoldoutGroup("粒子系统设置")]
        [LabelText("持续时间")]
        public float Duration = 0f;

        [FoldoutGroup("粒子系统设置")]
        [LabelText("初始化时停止系统")]
        public bool StopSystemOnInit = true;

        [FoldoutGroup("粒子系统设置")]
        [LabelText("播放时激活")]
        public bool ActivateOnPlay = false;

        [FoldoutGroup("播放设置")]
        [LabelText("模式")]
        public Modes Mode = Modes.Play;

        [FoldoutGroup("播放设置")]
        [LabelText("发射数量")]
        public int EmitCount = 100;

        [FoldoutGroup("播放设置")]
        [LabelText("包括子粒子系统")]
        public bool WithChildrenParticleSystems = true;

        public override string Tip => "播放场景中指定的粒子系统";

        private ParticleSystem.EmitParams _emitParams;

        private bool ShowRandomTargets => Target == null;
        private bool ShowTargetError => Target == null && RandomTargets.IsNullOrEmpty();


        public override float GetDuration()
        {
            return Duration;
        }

        protected override void OnFeedbackInit()
        {
            if (StopSystemOnInit)
            {
                StopParticles();
            }
        }

        protected override void OnFeedbackPlay()
        {
            PlayParticles();
        }

        protected override void OnFeedbackStop()
        {
            StopParticles();
        }

        private void PlayParticles()
        {
            if (ActivateOnPlay)
            {
                Target.gameObject.SetActive(true);

                foreach (var particle in RandomTargets)
                {
                    particle.gameObject.SetActive(true);
                }
            }

            if (RandomTargets?.Length > 0)
            {
                int random = Random.Range(0, RandomTargets.Length);
                HandleParticleSystemAction(RandomTargets[random]);
            }
            else if (Target != null)
            {
                HandleParticleSystemAction(Target);
            }
        }

        private void HandleParticleSystemAction(ParticleSystem targetParticleSystem)
        {
            switch (Mode)
            {
                case Modes.Play:
                    if (targetParticleSystem != null)
                        targetParticleSystem.Play(WithChildrenParticleSystems);
                    break;
                case Modes.Emit:
                    _emitParams.applyShapeToPosition = true;
                    if (targetParticleSystem != null)
                        targetParticleSystem.Emit(_emitParams, EmitCount);
                    break;
                case Modes.Stop:
                    if (targetParticleSystem != null)
                        targetParticleSystem.Stop(WithChildrenParticleSystems);
                    break;
                case Modes.Pause:
                    if (targetParticleSystem != null)
                        targetParticleSystem.Pause(WithChildrenParticleSystems);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StopParticles()
        {
            foreach (var particle in RandomTargets)
            {
                particle.Stop();
            }

            if (Target != null)
            {
                Target.Stop();
            }
        }
    }
}
