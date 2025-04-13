using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EasyFramework.ToolKit
{
    [AddFeedbackMenu("音效/播放音频源")]
    public class AudioSourceFeedback : AbstractFeedback
    {
        public enum Modes
        {
            Play,
            Pause,
            UnPause,
            Stop
        }

        public AudioSource TargetAudioSource;
        public Modes Mode = Modes.Play;
        public AudioClip[] RandomSfx = Array.Empty<AudioClip>();
        public float MinVolume = 1f;
        public float MaxVolume = 1f;
        public float MinPitch = 1f;
        public float MaxPitch = 1f;

        public override string Tip => "播放场景中指定的AudioSource";

        private AudioClip _randomClip;
        private float _duration;

        public override float GetDuration()
        {
            return _duration;
        }

        protected override void OnFeedbackPlay()
        {
            switch (Mode)
            {
                case Modes.Play:
                    if (RandomSfx?.Length > 0)
                    {
                        _randomClip = RandomSfx[Random.Range(0, RandomSfx.Length)];
                        TargetAudioSource.clip = _randomClip;
                    }

                    float volume = Random.Range(MinVolume, MaxVolume);
                    float pitch = Random.Range(MinPitch, MaxPitch);
                    _duration = TargetAudioSource.clip.length;
                    PlayAudioSource(TargetAudioSource, volume, pitch);
                    break;

                case Modes.Pause:
                    _duration = 0.1f;
                    TargetAudioSource.Pause();
                    break;

                case Modes.UnPause:
                    _duration = 0.1f;
                    TargetAudioSource.UnPause();
                    break;

                case Modes.Stop:
                    _duration = 0.1f;
                    TargetAudioSource.Stop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PlayAudioSource(AudioSource audioSource, float volume, float pitch)
        {
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.timeSamples = 0;

            audioSource.Play();
        }

        protected override void OnFeedbackStop()
        {
            TargetAudioSource?.Stop();
        }
    }
}
