using System;
using Sirenix.OdinInspector;
using UnityEngine.Audio;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EasyGameFramework
{
    [EasyFeedbackHelper("播放场景中指定的AudioSource")]
    [AddEasyFeedbackMenu("音效/播放音效源")]
    public class EF_AudioSource : AbstractEasyFeedback
    {
        public enum Modes { Play, Pause, UnPause, Stop }

        [FoldoutGroup("Audio Source")]
        [Required]
        public AudioSource TargetAudioSource;
        [FoldoutGroup("Audio Source")]
        public Modes Mode = Modes.Play;

        [FoldoutGroup("Audio Source")]
        [TitleCN("Random Sound")]
        public AudioClip[] RandomSfx = Array.Empty<AudioClip>();

        [FoldoutGroup("Audio Settings")]
        [TitleCN("Volume")]
        public float MinVolume = 1f;
        [FoldoutGroup("Audio Settings")]
        public float MaxVolume = 1f;

        [FoldoutGroup("Audio Settings")]
        [TitleCN("Pitch")]
        public float MinPitch = 1f;
        [FoldoutGroup("Audio Settings")]
        public float MaxPitch = 1f;

        [FoldoutGroup("Audio Settings")]
        [TitleCN("Mixer")]
        public AudioMixerGroup SfxAudioMixerGroup;

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
