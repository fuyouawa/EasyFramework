using System;
using System.Collections;
using EasyFramework.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.Tools
{
    [AddEasyFeedbackMenu("精灵/帧动画")]
    public class EF_SpriteFrameAnimation : AbstractEasyFeedback
    {
        public enum DelayModes
        {
            Frame,
            Seconds
        }

        [Serializable]
        public struct Frame
        {

            public Sprite Sprite;
            public DelayModes DelayMode;
            [ShowIf("DelayMode", DelayModes.Frame)]
            public int DelayFrames;
            [ShowIf("DelayMode", DelayModes.Seconds)]
            public float DelaySeconds;

            public IEnumerator DelayCo()
            {
                switch (DelayMode)
                {
                    case DelayModes.Frame:
                        for (int i = 0; i < DelayFrames; i++)
                        {
                            yield return null;
                        }
                        break;
                    case DelayModes.Seconds:
                        yield return new WaitForSeconds(DelaySeconds);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override IEnumerator Pause => PlayAtlasCo();

        [FoldoutGroup("Frame Animation")]
        public SpriteRenderer SpriteRenderer;
        [FoldoutGroup("Frame Animation")]
        [ListDrawerSettings(ShowIndexLabels = true, CustomAddFunction = "ConstructNewFrame")]
        public Frame[] Frames = Array.Empty<Frame>();

        [FoldoutGroup("Frames Settings")]
        [TitleCN("Default New Frame")]
        [LabelText("Delay Mode")]
        public DelayModes DefaultDelayMode;

        [FoldoutGroup("Frames Settings")]
        [ShowIf("DefaultDelayMode", DelayModes.Frame)]
        [LabelText("Delay Frames")]
        public int DefaultDelayFrames = 6;

        [FoldoutGroup("Frames Settings")]
        [ShowIf("DefaultDelayMode", DelayModes.Seconds)]
        [LabelText("Delay Seconds")]
        public float DefaultDelaySeconds = 0.1f;

        [FoldoutGroup("Frames Settings")]
        [TitleCN("Batch Modifying")]
        public int BeginIndex;
        [FoldoutGroup("Frames Settings")]
        public int EndIndex;

        [FoldoutGroup("Frames Settings")]
        public DelayModes DelayModeToModify;

        [FoldoutGroup("Frames Settings")]
        [ShowIf("DelayModeToModify", DelayModes.Frame)]
        public int DelayFramesToModify = 6;

        [FoldoutGroup("Frames Settings")]
        [ShowIf("DelayModeToModify", DelayModes.Seconds)]
        public float DelaySecondsToModify = 0.1f;

        [FoldoutGroup("Frames Settings")]
        [Button]
        public void BatchModify()
        {
            for (int i = BeginIndex; i <= EndIndex; i++)
            {
                var f = Frames[i];
                f.DelayMode = DelayModeToModify;
                f.DelaySeconds = DelaySecondsToModify;
                f.DelayFrames = DelayFramesToModify;
                Frames[i] = f;
            }
        }
        
        public override string Tip => "在指定的SpriteRenderer上循环赋值图集中的精灵, 实现帧动画的效果";

        protected override void OnFeedbackPlay()
        {
        }

        private IEnumerator PlayAtlasCo()
        {
            foreach (var frame in Frames)
            {
                SpriteRenderer.sprite = frame.Sprite;
                yield return frame.DelayCo();
            }
        }

        protected override void OnFeedbackStop()
        {
        }

#if UNITY_EDITOR
        private Frame ConstructNewFrame()
        {
            return new Frame()
            {
                DelayMode = DefaultDelayMode,
                DelaySeconds = DefaultDelaySeconds,
                DelayFrames = DefaultDelayFrames
            };
        }
#endif
    }
}
