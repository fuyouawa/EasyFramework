using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework.Generic;
using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    internal class EasyFeedbackHelper
    {
        private static FieldInfo[] s_ignoreDrawFields;
        public static FieldInfo[] IgnoreDrawFields => s_ignoreDrawFields ??= typeof(AbstractEasyFeedback).GetFields().ToArray();
    }

    public class AbstractEasyFeedbackDrawer<T> : FoldoutValueDrawer<T>
        where T : AbstractEasyFeedback
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
            var attr = Property.GetAttribute<AddEasyFeedbackMenuAttribute>();
            _labelConfig.Content.text = $"[{attr.Path.Replace("/", " - ")}]";
            return _labelConfig;
        }

        protected override GUIContent GetLabel(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            return new GUIContent("      " + (value.Label.IsNullOrEmpty() ? "TODO" : value.Label));
        }

        protected class PropertiesGroupInfo
        {
            public GUIContent Label = new GUIContent();
            public OnContentGUIDelegate OnContentGUI;

            public PropertiesGroupInfo(string label, OnContentGUIDelegate onContentGui)
            {
                Label.text = label;
                OnContentGUI = onContentGui;
            }

            public PropertiesGroupInfo(GUIContent label, OnContentGUIDelegate onContentGui)
            {
                Label = label;
                OnContentGUI = onContentGui;
            }
        }

        private readonly List<InspectorProperty> _properties = new List<InspectorProperty>();
        private readonly List<FoldoutGroupConfig> _foldoutGroupConfigs = new List<FoldoutGroupConfig>();

        protected List<PropertiesGroupInfo> PropertiesGroups = new List<PropertiesGroupInfo>();

        protected T Feedback { get; private set; }

        protected override void Initialize()
        {
            base.Initialize();
            Feedback = ValueEntry.SmartValue;

            PropertiesGroups.Add(new PropertiesGroupInfo("反馈设置", OnSettingsContentGUI));
            PostBuildPropertiesGroups();

            for (int i = 0; i < PropertiesGroups.Count; i++)
            {
                var prop = Property.Children[i];
                var info = PropertiesGroups[i];

                _properties.Add(prop);
                _foldoutGroupConfigs.Add(new FoldoutGroupConfig(
                    UniqueDrawerKey.Create(prop, this),
                    info.Label, true, info.OnContentGUI));
            }

            var style = new GUIStyle(SirenixGUIStyles.BoldLabel);
            style.fontSize += 1;
            style.padding.bottom += 2;
            style.padding.left -= 9;
            _labelConfig.Style = style;
        }

        protected virtual void PostBuildPropertiesGroups()
        {
            var label = new GUIContent();
            if (Feedback.GroupName.IsNullOrEmpty())
            {
                var attr = Property.GetAttribute<AddEasyFeedbackMenuAttribute>();
                label.text = attr.Path.Split('/').Last();
            }
            else
            {
                label.text = Feedback.GroupName;
            }

            PropertiesGroups.Add(new PropertiesGroupInfo(label, rect =>
            {
                foreach (var child in Property.Children)
                {
                    if (!Array.Exists(EasyFeedbackHelper.IgnoreDrawFields, f => f.Name == child.Name))
                    {
                        child.Draw();
                    }
                }
            }));
        }

        private void OnSettingsContentGUI(Rect headerRect)
        {
            EasyEditorField.Value(
                EditorHelper.TempContent("播放前延迟", "在正式Play前经过多少时间的延迟(s)"),
                ref Feedback.DelayBeforePlay);
            EasyEditorField.Value(
                EditorHelper.TempContent("阻塞", "是否会阻塞反馈运行"),
                ref Feedback.Blocking);
            EasyEditorField.Value(
                EditorHelper.TempContent("无限重复", "无限重复播放"),
                ref Feedback.RepeatForever);

            if (!Feedback.RepeatForever)
            {
                EasyEditorField.Value(
                    EditorHelper.TempContent("重复次数", "重复播放的次数"),
                    ref Feedback.AmountOfRepeat);
            }

            EasyEditorField.Value(
                EditorHelper.TempContent("重复间隔", "每次循环播放的间隔"),
                ref Feedback.IntervalBetweenRepeats);
        }

        protected override void OnContentGUI(Rect headerRect)
        {
            if (Feedback.Tip.IsNotNullOrWhiteSpace())
            {
                EasyEditorGUI.MessageBox(Feedback.Tip, MessageType.Info);
            }

            EasyEditorField.Value(
                EditorHelper.TempContent("标签"),
                ref Feedback.Label);
            EasyEditorField.Value(
                new GUIContent("启用"),
                ref Feedback.Enable);

            DrawPropertiesGroups();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("播放"))
            {
                Feedback.StartCoroutine(Feedback.PlayCo());
            }

            if (GUILayout.Button("重置"))
            {
                Feedback.Reset();
            }

            if (GUILayout.Button("停止"))
            {
                Feedback.Stop();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        private void DrawPropertiesGroups()
        {
            for (int i = 0; i < PropertiesGroups.Count; i++)
            {
                FoldoutGroup(_foldoutGroupConfigs[i], _properties[i]);
            }

            return;

            void FoldoutGroup(FoldoutGroupConfig config, InspectorProperty property)
            {
                config.Expand = property.State.Expanded;
                property.State.Expanded = EasyEditorGUI.FoldoutGroup(config);
            }
        }
    }
}
