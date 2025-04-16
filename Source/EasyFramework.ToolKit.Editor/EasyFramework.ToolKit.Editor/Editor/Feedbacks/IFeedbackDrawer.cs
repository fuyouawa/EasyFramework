using EasyFramework.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class IFeedbackDrawer<T> : FoldoutValueDrawer<T>
        where T : IFeedback
    {
        private static readonly float IconWidth = EditorGUIUtility.singleLineHeight;

        protected override void OnCoveredTitleBarGUI(Rect headerRect)
        {
            var value = ValueEntry.SmartValue;

            var buttonRect = new Rect(headerRect)
            {
                x = headerRect.x + 17,
                width = IconWidth,
                height = IconWidth
            };

            value.Enable = EditorGUI.Toggle(buttonRect, value.Enable);
        }

        private readonly LabelConfig _labelConfig = new LabelConfig(
            new GUIContent(),
            Color.yellow);

        protected override LabelConfig GetRightLabelConfig(GUIContent label)
        {
            var attr = Property.GetAttribute<AddFeedbackMenuAttribute>();
            _labelConfig.Content.text = $"[{attr.Path.Replace("/", " - ")}]";
            return _labelConfig;
        }

        protected override GUIContent GetLabel(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            return new GUIContent("      " + (value.Label.IsNullOrEmpty() ? "TODO" : value.Label));
        }

        protected override void Initialize()
        {
            base.Initialize();

            var style = new GUIStyle(SirenixGUIStyles.BoldLabel);
            style.fontSize += 1;
            style.padding.bottom += 2;
            style.padding.left -= 9;
            _labelConfig.Style = style;
        }

        private void OnSettingsContentGUI(Rect headerRect)
        {
            // _propertyOfDelayBeforePlay.DrawEx("播放前延迟", "在正式Play前经过多少时间的延迟(s)");
            // _propertyOfBlocking.DrawEx("阻塞", "是否会阻塞反馈运行");
            // _propertyOfRepeatForever.DrawEx("无限重复", "无限重复播放");
            //
            // _propertyOfAmountOfRepeat.DrawEx("重复次数", "重复播放的次数");
            // _propertyOfIntervalBetweenRepeats.DrawEx("重复间隔", "每次循环播放的间隔");
        }

        // protected override void OnContentGUI(Rect headerRect)
        // {
        //     if (Feedback.Tip.IsNotNullOrWhiteSpace())
        //     {
        //         EasyEditorGUI.MessageBox(Feedback.Tip, MessageType.Info);
        //     }
        //     
        //     _propertyOfLabel.DrawEx("标签");
        //     _propertyOfEnable.DrawEx("启用");
        //
        //     DrawPropertiesGroups();
        //     if (_autoDrawOtherProperties)
        //     {
        //         DrawOtherProperties();
        //     }
        //
        //     EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        //     EditorGUILayout.BeginHorizontal();
        //
        //     if (GUILayout.Button("播放"))
        //     {
        //         Feedback.StartCoroutine(Feedback.PlayCo());
        //     }
        //
        //     if (GUILayout.Button("重置"))
        //     {
        //         Feedback.Reset();
        //     }
        //
        //     if (GUILayout.Button("停止"))
        //     {
        //         Feedback.Stop();
        //     }
        //
        //     EditorGUILayout.EndHorizontal();
        //     EditorGUI.EndDisabledGroup();
        // }
    }
}
