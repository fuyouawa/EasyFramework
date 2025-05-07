using System;
using EasyFramework.Core;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EasyFramework.Modules
{
    [AddFeedbackMenu("音效/播放声音")]
    public class SoundPlayFeedback : AbstractFeedback
    {
        public enum PlayMethods
        {
            Cached,
            OnDemand,
            Pool
        }

        [FoldoutGroup("音频设置")]
        [InfoBoxEx("音频不能为空！", InfoMessageType.Error, nameof(ShowSfxError))]
        [LabelText("音频")]
        public AudioClip Sfx;

        [FoldoutGroup("音频设置")]
        [ShowIf(nameof(ShowRandomSfx))]
        [LabelText("随机音频")]
        public AudioClip[] RandomSfx = Array.Empty<AudioClip>();

        // 播放方式
        
        [FoldoutGroup("播放方式")]
        [LabelText("播放方式")]
        public PlayMethods PlayMethod = PlayMethods.Cached;

        [FoldoutGroup("播放方式")]
        [ShowIf(nameof(PlayMethod), PlayMethods.Pool)]
        [LabelText("池大小")]
        public int PoolSize = 10;

        [FoldoutGroup("播放方式")]
        [LabelText("反馈停止时停止音效")]
        public bool StopSoundOnFeedbackStop = true;

        // 声音属性
        
        [FoldoutGroup("声音属性")]
        [LabelText("响度")]
        public SliderValue Volume = new SliderValue(1f);

        [FoldoutGroup("声音属性")]
        [LabelText("音高")]
        public SliderValue Pitch = new SliderValue(1f);

        [FoldoutGroup("声音属性")]
        [LabelText("混音器组")]
        public AudioMixerGroup SfxAudioMixerGroup;

        [FoldoutGroup("声音属性")]
        [LabelText("优先级")]
        public int Priority = 128;

        // 空间设置
        
        [FoldoutGroup("空间设置")]
        [Range(-1f, 1f)]
        [LabelText("立体声平衡")]
        public float PanStereo;

        [FoldoutGroup("空间设置")]
        [Range(0f, 1f)]
        [LabelText("空间感")]
        public float SpatialBlend;

        // 3D Sound Settings
        
        [FoldoutGroup("3D声音设置")]
        [Range(0f, 5f)]
        [LabelText("多普勒效应")]
        public float DopplerLevel = 1f;

        [FoldoutGroup("3D声音设置")]
        [Range(0, 360)]
        [LabelText("扩展度")]
        public int Spread = 0;
        
        [FoldoutGroup("3D声音设置")]
        [LabelText("衰减方式")]
        public AudioRolloffMode RolloffMode = AudioRolloffMode.Logarithmic;

        [FoldoutGroup("3D声音设置")]
        [LabelText("距离")]
        public SliderValue Distance = new SliderValue(new Vector2(1f, 500f));
        
        [FoldoutGroup("3D声音设置")]
        [LabelText("启用自定义衰减曲线")]
        public bool UseCustomRolloffCurve = false;

        [FoldoutGroup("3D声音设置")]
        [ShowIf(nameof(UseCustomRolloffCurve))]
        [LabelText("自定义衰减曲线")]
        public AnimationCurve CustomRolloffCurve;
        
        [FoldoutGroup("3D声音设置")]
        [LabelText("启用自定义空间混合曲线")]
        public bool UseSpatialBlendCurve = false;

        [FoldoutGroup("3D声音设置")]
        [ShowIf(nameof(UseSpatialBlendCurve))]
        [LabelText("自定义空间混合曲线")]
        public AnimationCurve SpatialBlendCurve;
        
        [FoldoutGroup("3D声音设置")]
        [LabelText("启用自定义混响区混合曲线")]
        public bool UseReverbZoneMixCurve = false;

        [FoldoutGroup("3D声音设置")]
        [ShowIf(nameof(UseReverbZoneMixCurve))]
        [LabelText("自定义混响区混合曲线")]
        public AnimationCurve ReverbZoneMixCurve;
        
        [FoldoutGroup("3D声音设置")]
        [LabelText("启用自定义扩展曲线")]
        public bool UseSpreadCurve = false;

        [FoldoutGroup("3D声音设置")]
        [ShowIf(nameof(UseSpreadCurve))]
        [LabelText("自定义扩展曲线")]
        public AnimationCurve SpreadCurve;

        public override string Tip => "配置一个声音用于播放, 在初始化时会自动创建对应的AudioSource";
        
        private bool ShowRandomSfx => Sfx == null;
        private bool ShowSfxError => Sfx == null && RandomSfx.IsNullOrEmpty();
        
        private AudioClip _randomClip;
        private AudioSource _cachedAudioSource;
        private AudioSource[] _pool;
        private AudioSource _tempAudioSource;
        private float _duration;
        private AudioSource _audioSource;


        protected override void OnFeedbackInit()
        {
            if (PlayMethod == PlayMethods.Cached)
            {
                _cachedAudioSource = CreateAudioSource(Owner.gameObject, "CachedFeedbackAudioSource");
            }

            if (PlayMethod == PlayMethods.Pool)
            {
                // create a pool
                _pool = new AudioSource[PoolSize];
                for (int i = 0; i < PoolSize; i++)
                {
                    _pool[i] = CreateAudioSource(Owner.gameObject, "PooledAudioSource" + i);
                }
            }
        }

        private AudioSource CreateAudioSource(GameObject owner, string audioSourceName)
        {
            // we create a temporary game object to host our audio source
            GameObject temporaryAudioHost = new GameObject(audioSourceName);
            SceneManager.MoveGameObjectToScene(temporaryAudioHost.gameObject, Owner.gameObject.scene);
            // we set the temp audio's position
            temporaryAudioHost.transform.position = owner.transform.position;
            temporaryAudioHost.transform.SetParent(owner.transform);
            // we add an audio source to that host
            _tempAudioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
            _tempAudioSource.playOnAwake = false;
            return _tempAudioSource;
        }

        protected override void OnFeedbackPlay()
        {
            if (Sfx != null)
            {
                _duration = Sfx.length;
                PlaySound(Sfx, Owner.transform.position);
                return;
            }

            if (RandomSfx.Length > 0)
            {
                _randomClip = RandomSfx[Random.Range(0, RandomSfx.Length)];

                if (_randomClip != null)
                {
                    _duration = _randomClip.length;
                    PlaySound(_randomClip, Owner.transform.position);
                }
            }
        }

        public override float GetDuration()
        {
            if (Sfx != null)
            {
                return Sfx.length;
            }

            float longest = 0f;
            if (RandomSfx.IsNotNullOrEmpty())
            {
                foreach (var clip in RandomSfx)
                {
                    if ((clip != null) && (clip.length > longest))
                    {
                        longest = clip.length;
                    }
                }

                return longest;
            }

            return 0f;
        }

        /// <summary>
        /// Plays a sound differently based on the selected play method
        /// </summary>
        /// <param name="sfx"></param>
        /// <param name="position"></param>
        private void PlaySound(AudioClip sfx, Vector3 position)
        {
            float volume = Volume.Evaluate();

            float pitch = Pitch.Evaluate();

            switch (PlayMethod)
            {
                case PlayMethods.Cached:
                    // we set that audio source clip to the one in paramaters
                    PlayAudioSource(_cachedAudioSource, sfx, volume, pitch, SfxAudioMixerGroup, Priority);
                    break;
                case PlayMethods.OnDemand:
                    // we create a temporary game object to host our audio source
                    GameObject temporaryAudioHost = new GameObject("TempAudio");
                    SceneManager.MoveGameObjectToScene(temporaryAudioHost.gameObject, Owner.gameObject.scene);
                    // we set the temp audio's position
                    temporaryAudioHost.transform.position = position;
                    // we add an audio source to that host
                    AudioSource audioSource = temporaryAudioHost.AddComponent<AudioSource>();
                    PlayAudioSource(audioSource, sfx, volume, pitch, SfxAudioMixerGroup, Priority);
                    // we destroy the host after the clip has played
                    GameObject.Destroy(temporaryAudioHost, sfx.length);
                    break;
                case PlayMethods.Pool:
                    _tempAudioSource = GetAudioSourceFromPool();
                    if (_tempAudioSource != null)
                    {
                        PlayAudioSource(_tempAudioSource, sfx, volume, pitch, SfxAudioMixerGroup, Priority);
                    }

                    break;
            }
        }

        protected override void OnFeedbackStop()
        {
            if (_audioSource != null)
            {
                if (StopSoundOnFeedbackStop)
                {
                    _audioSource.Stop();
                }
            }
        }

        /// <summary>
        /// Plays the audio source with the specified volume and pitch
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="sfx"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        private void PlayAudioSource(AudioSource audioSource, AudioClip sfx, float volume, float pitch,
            AudioMixerGroup audioMixerGroup = null, int priority = 128)
        {
            _audioSource = audioSource;
            // we set that audio source clip to the one in paramaters
            audioSource.clip = sfx;
            audioSource.timeSamples = 0;
            // we set the audio source volume to the one in parameters
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.priority = priority;
            // we set spatial settings
            audioSource.panStereo = PanStereo;
            audioSource.spatialBlend = SpatialBlend;
            audioSource.dopplerLevel = DopplerLevel;
            audioSource.spread = Spread;
            audioSource.rolloffMode = RolloffMode;
            audioSource.minDistance = Distance.Min;
            audioSource.maxDistance = Distance.Max;
            if (UseSpreadCurve)
            {
                audioSource.SetCustomCurve(AudioSourceCurveType.Spread, SpreadCurve);
            }

            if (UseCustomRolloffCurve)
            {
                audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, CustomRolloffCurve);
            }

            if (UseSpatialBlendCurve)
            {
                audioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, SpatialBlendCurve);
            }

            if (UseReverbZoneMixCurve)
            {
                audioSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, ReverbZoneMixCurve);
            }

            // we set our loop setting
            audioSource.loop = false;
            if (audioMixerGroup != null)
            {
                audioSource.outputAudioMixerGroup = audioMixerGroup;
            }

            // we start playing the sound
            audioSource.Play();
        }

        /// <summary>
        /// Gets an audio source from the pool if possible
        /// </summary>
        /// <returns></returns>
        private AudioSource GetAudioSourceFromPool()
        {
            for (int i = 0; i < PoolSize; i++)
            {
                if (!_pool[i].isPlaying)
                {
                    return _pool[i];
                }
            }

            return null;
        }
    }
}
