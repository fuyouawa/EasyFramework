using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EasyGameFramework
{
    [AddEasyFeedbackMenu("粒子/播放粒子")]
    public class EF_ParticlePlay : AbstractEasyFeedback
    {
        public enum Modes { Play, Stop, Pause, Emit }

        [FoldoutGroup("Bound Particles")]
        public Modes Mode = Modes.Play;

        [FoldoutGroup("Bound Particles")]
        [ShowIf("Mode", Modes.Emit)]
        public int EmitCount = 100;

        [FoldoutGroup("Bound Particles")]
        public ParticleSystem BoundParticleSystem;
        [FoldoutGroup("Bound Particles")]
        public bool WithChildrenParticles = true;
        [FoldoutGroup("Bound Particles")]
        [HideIf("@BoundParticleSystem != null")]
        public ParticleSystem[] RandomParticleSystems = Array.Empty<ParticleSystem>();
        [FoldoutGroup("Bound Particles")]
        public bool ActivateOnPlay = false;
        [FoldoutGroup("Bound Particles")]
        public bool StopSystemOnInit = true;
        [FoldoutGroup("Bound Particles")]
        public float DeclaredDuration = 0f;

        [FoldoutGroup("Simulation Speed")]
        public bool ForceSimulationSpeed = false;
        [FoldoutGroup("Simulation Speed")]
        [ShowIf("ForceSimulationSpeed")]
        public Vector2 ForcedSimulationSpeed = new Vector2(0.1f, 1f);
        
        public override string Tip => "播放场景中指定的粒子系统";

        private ParticleSystem.EmitParams _emitParams;

        

        public override float GetDuration()
        {
            return DeclaredDuration;
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
                BoundParticleSystem.gameObject.SetActive(true);

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
            else if (BoundParticleSystem != null)
            {
                HandleParticleSystemAction(BoundParticleSystem);
            }
        }
        private void HandleParticleSystemAction(ParticleSystem targetParticleSystem)
        {
            if (ForceSimulationSpeed)
            {
                var main = targetParticleSystem.main;
                main.simulationSpeed = Random.Range(ForcedSimulationSpeed.x, ForcedSimulationSpeed.y);
            }

            switch (Mode)
            {
                case Modes.Play:
                    if (targetParticleSystem != null)
                        targetParticleSystem.Play(WithChildrenParticles);
                    break;
                case Modes.Emit:
                    _emitParams.applyShapeToPosition = true;
                    if (targetParticleSystem != null)
                        targetParticleSystem.Emit(_emitParams, EmitCount);
                    break;
                case Modes.Stop:
                    if (targetParticleSystem != null)
                        targetParticleSystem.Stop(WithChildrenParticles);
                    break;
                case Modes.Pause:
                    if (targetParticleSystem != null)
                        targetParticleSystem.Pause(WithChildrenParticles);
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
            if (BoundParticleSystem != null)
            {
                BoundParticleSystem.Stop();
            }
        }
    }
}
