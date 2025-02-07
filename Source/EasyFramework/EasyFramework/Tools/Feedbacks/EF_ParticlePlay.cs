using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EasyFramework.Feedbacks
{
    [AddEasyFeedbackMenu("粒子/播放粒子")]
    public class EF_ParticlePlay : AbstractEasyFeedback
    {
        public enum Modes
        {
            Play,
            Stop,
            Pause,
            Emit
        }

        public ParticleSystem TargetParticleSystem;
        public Modes Mode = Modes.Play;
        public int EmitCount = 100;
        public bool WithChildrenParticleSystems = true;
        public ParticleSystem[] RandomParticleSystems = Array.Empty<ParticleSystem>();
        public bool ActivateOnPlay = false;
        public bool StopSystemOnInit = true;
        public float Duration = 0f;

        public override string Tip => "播放场景中指定的粒子系统";

        private ParticleSystem.EmitParams _emitParams;


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
                TargetParticleSystem.gameObject.SetActive(true);

                foreach (var particle in RandomParticleSystems)
                {
                    particle.gameObject.SetActive(true);
                }
            }

            if (RandomParticleSystems?.Length > 0)
            {
                int random = Random.Range(0, RandomParticleSystems.Length);
                HandleParticleSystemAction(RandomParticleSystems[random]);
            }
            else if (TargetParticleSystem != null)
            {
                HandleParticleSystemAction(TargetParticleSystem);
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
            foreach (var particle in RandomParticleSystems)
            {
                particle.Stop();
            }

            if (TargetParticleSystem != null)
            {
                TargetParticleSystem.Stop();
            }
        }
    }
}
