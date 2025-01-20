using System;
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

        private InspectorProperty _property;

        private InspectorProperty _freeProperty1;
        private InspectorProperty _freeProperty2;
        private InspectorProperty _freeProperty3;
        private InspectorProperty _freeProperty4;
        private InspectorProperty _freeProperty5;
        private InspectorProperty _freeProperty6;

        protected bool FreeExpand1
        {
            get => _freeProperty1.State.Expanded;
            set => _freeProperty1.State.Expanded = value;
        }

        protected bool FreeExpand2
        {
            get => _freeProperty2.State.Expanded;
            set => _freeProperty2.State.Expanded = value;
        }

        protected bool FreeExpand3
        {
            get => _freeProperty3.State.Expanded;
            set => _freeProperty3.State.Expanded = value;
        }

        protected bool FreeExpand4
        {
            get => _freeProperty4.State.Expanded;
            set => _freeProperty4.State.Expanded = value;
        }

        protected bool FreeExpand5
        {
            get => _freeProperty5.State.Expanded;
            set => _freeProperty5.State.Expanded = value;
        }

        protected bool FreeExpand6
        {
            get => _freeProperty6.State.Expanded;
            set => _freeProperty6.State.Expanded = value;
        }


        protected object FreeKey1 { get; private set; }
        protected object FreeKey2 { get; private set; }
        protected object FreeKey3 { get; private set; }
        protected object FreeKey4 { get; private set; }
        protected object FreeKey5 { get; private set; }
        protected object FreeKey6 { get; private set; }

        protected T Feedback { get; private set; }

        protected override void Initialize()
        {
            base.Initialize();
            _property = Property.Children[nameof(AbstractEasyFeedback.Label)];
            _freeProperty1 = Property.Children[nameof(AbstractEasyFeedback.Enable)];
            _freeProperty2 = Property.Children[nameof(AbstractEasyFeedback.DelayBeforePlay)];
            _freeProperty3 = Property.Children[nameof(AbstractEasyFeedback.Blocking)];
            _freeProperty4 = Property.Children[nameof(AbstractEasyFeedback.RepeatForever)];
            _freeProperty5 = Property.Children[nameof(AbstractEasyFeedback.AmountOfRepeat)];
            _freeProperty6 = Property.Children[nameof(AbstractEasyFeedback.IntervalBetweenRepeats)];

            FreeKey1 = UniqueDrawerKey.Create(_freeProperty1, this);
            FreeKey2 = UniqueDrawerKey.Create(_freeProperty2, this);
            FreeKey3 = UniqueDrawerKey.Create(_freeProperty3, this);
            FreeKey4 = UniqueDrawerKey.Create(_freeProperty4, this);
            FreeKey5 = UniqueDrawerKey.Create(_freeProperty5, this);
            FreeKey6 = UniqueDrawerKey.Create(_freeProperty6, this);

            Feedback = ValueEntry.SmartValue;

            var style = new GUIStyle(SirenixGUIStyles.BoldLabel);
            style.fontSize += 1;
            style.padding.bottom += 2;
            style.padding.left -= 9;
            _labelConfig.Style = style;
        }

        protected override void OnContentGUI(Rect headerRect)
        {
            var value = ValueEntry.SmartValue;

            if (value.Tip.IsNotNullOrWhiteSpace())
            {
                EasyEditorGUI.MessageBox(value.Tip, MessageType.Info);
            }

            value.Label = EasyEditorField.Value(
                EditorHelper.TempContent("标签"),
                value.Label);
            value.Enable = EasyEditorField.Value(
                new GUIContent("启用"),
                value.Enable);

            _property.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_property, this),
                "反馈设置",
                _property.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    value.DelayBeforePlay = EasyEditorField.Value(
                        EditorHelper.TempContent("播放前延迟", "在正式Play前经过多少时间的延迟(s)"),
                        value.DelayBeforePlay);
                    value.Blocking = EasyEditorField.Value(
                        EditorHelper.TempContent("阻塞", "是否会阻塞反馈运行"),
                        value.Blocking);
                    value.RepeatForever = EasyEditorField.Value(
                        EditorHelper.TempContent("无限重复", "无限重复播放"),
                        value.RepeatForever);

                    if (!value.RepeatForever)
                    {
                        value.AmountOfRepeat = EasyEditorField.Value(
                            EditorHelper.TempContent("重复次数", "重复播放的次数"),
                            value.AmountOfRepeat);
                    }

                    value.IntervalBetweenRepeats = EasyEditorField.Value(
                        EditorHelper.TempContent("重复间隔", "每次循环播放的间隔"),
                        value.IntervalBetweenRepeats);
                },
            });

            DrawOtherPropertyLayout();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("播放"))
            {
                value.StartCoroutine(value.PlayCo());
            }

            if (GUILayout.Button("重置"))
            {
                value.Reset();
            }

            if (GUILayout.Button("停止"))
            {
                value.Stop();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        private static FieldInfo[] s_ignoreDrawFields;

        private static FieldInfo[] IgnoreDrawFields
        {
            get
            {
                if (s_ignoreDrawFields == null)
                {
                    s_ignoreDrawFields = typeof(AbstractEasyFeedback).GetFields().ToArray();
                }

                return s_ignoreDrawFields;
            }
        }

        protected virtual void DrawOtherPropertyLayout()
        {
            foreach (var child in Property.Children)
            {
                if (!Array.Exists(IgnoreDrawFields, f => f.Name == child.Name))
                {
                    child.Draw();
                }
            }
        }
    }
}
