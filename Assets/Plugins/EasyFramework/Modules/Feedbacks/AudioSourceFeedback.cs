using System;
using EasyFramework.Core;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EasyFramework.Modules
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

        [FoldoutGroup("音频源设置")]
        [InfoBoxEx("音频源不能为空！", InfoMessageType.Error, nameof(ShowTargetError))]
        [LabelText("音频源")]
        public AudioSource Target;

        [FoldoutGroup("音频源设置")]
        [LabelText("音频源")]
        public Modes Mode = Modes.Play;

        [FoldoutGroup("音频源设置")]
        [LabelText("随机音频")]
        public AudioClip[] RandomSfx = Array.Empty<AudioClip>();

        [FoldoutGroup("音频源设置")]
        [MinMaxSlider(0f, 1f)]
        [LabelText("响度范围")]
        public Vector2 VolumeRange = Vector2.one;

        [FoldoutGroup("音频源设置")]
        [MinMaxSlider(0f, 1f)]
        [LabelText("音高范围")]
        public Vector2 PitchRange = Vector2.one;

        public override string Tip => "播放场景中指定的AudioSource";

        private AudioClip _randomClip;
        private float _duration;

        private bool ShowTargetError => Target == null && RandomSfx.IsNullOrEmpty();

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
                        Target.clip = _randomClip;
                    }

                    float volume = Random.Range(VolumeRange.x, VolumeRange.y);
                    float pitch = Random.Range(PitchRange.x, PitchRange.y);
                    _duration = Target.clip.length;
                    PlayAudioSource(Target, volume, pitch);
                    break;

                case Modes.Pause:
                    _duration = 0.1f;
                    Target.Pause();
                    break;

                case Modes.UnPause:
                    _duration = 0.1f;
                    Target.UnPause();
                    break;

                case Modes.Stop:
                    _duration = 0.1f;
                    Target.Stop();
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
            Target?.Stop();
        }
    }
}
