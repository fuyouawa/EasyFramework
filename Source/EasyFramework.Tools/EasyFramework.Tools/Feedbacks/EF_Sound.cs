using System;
using EasyFramework.Generic;
using EasyFramework.Utilities;
using Sirenix.OdinInspector;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EasyFramework.Tools
{
    [AddEasyFeedbackMenu("音效/播放声音")]
    public class EF_Sound : AbstractEasyFeedback
    {
        public enum PlayMethods
        {
            Cached,
            OnDemand,
            Pool
        }

        public AudioClip Sfx;
        public AudioClip[] RandomSfx = Array.Empty<AudioClip>();

        // 播放方式

        public PlayMethods PlayMethod = PlayMethods.Cached;
        public int PoolSize = 10;
        public bool StopSoundOnFeedbackStop = true;

        // 声音属性

        public float MinVolume = 1f;
        public float MaxVolume = 1f;
        public float MinPitch = 1f;
        public float MaxPitch = 1f;
        public AudioMixerGroup SfxAudioMixerGroup;
        public int Priority = 128;

        // 空间设置

        public float PanStereo;
        public float SpatialBlend;

        // 3d声音设置

        public float DopplerLevel = 1f;
        public int Spread = 0;

        public AudioRolloffMode RolloffMode = AudioRolloffMode.Logarithmic;
        public float MinDistance = 1f;
        public float MaxDistance = 500f;

        public bool UseCustomRolloffCurve = false;
        public AnimationCurve CustomRolloffCurve;

        public bool UseSpatialBlendCurve = false;
        public AnimationCurve SpatialBlendCurve;

        public bool UseReverbZoneMixCurve = false;
        public AnimationCurve ReverbZoneMixCurve;

        public bool UseSpreadCurve = false;
        public AnimationCurve SpreadCurve;

        public override string Tip => "配置一个声音用于播放, 在初始化时会自动创建对应的AudioSource";

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
            float volume = Random.Range(MinVolume, MaxVolume);

            float pitch = Random.Range(MinPitch, MaxPitch);

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
            audioSource.minDistance = MinDistance;
            audioSource.maxDistance = MaxDistance;
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
