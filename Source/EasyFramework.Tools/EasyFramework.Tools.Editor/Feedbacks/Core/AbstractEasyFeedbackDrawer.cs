using System;
using System.Linq;
using System.Reflection;
using EasyFramework;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Editor
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

        protected override string GetRightLabel(GUIContent label)
        {
            var attr = ValueEntry.SmartValue.GetType().GetCustomAttribute<AddEasyFeedbackMenuAttribute>();
            if (attr != null)
            {
                return $"[{attr.Path.Split("/").Last()}]";
            }

            return base.GetRightLabel(label);
        }

        protected override string GetLabel(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            return "      " + (value.Label.IsNullOrEmpty() ? "TODO" : value.Label);
        }

        private InspectorProperty _property;
        protected override void Initialize()
        {
            base.Initialize();
            _property = Property.Children["Label"];
        }

        protected override void OnContentGUI(Rect headerRect)
        {
            var value = ValueEntry.SmartValue;

            if (value.Tip.IsNotNullOrWhiteSpace())
            {
                EasyEditorGUI.MessageBox(value.Tip, MessageType.Info);
            }

            value.Label = EditorGUILayout.TextField(new GUIContent("标签"), value.Label);
            value.Enable = EditorGUILayout.Toggle(new GUIContent("激活"), value.Enable);

            _property.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_property, this),
                "Feedback设置",
                _property.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    value.DelayBeforePlay = EditorGUILayout.FloatField(
                        new GUIContent("播放前延迟", "在正式Play前经过多少时间的延迟(s)"),
                        value.DelayBeforePlay);
                    value.Blocking = EditorGUILayout.Toggle(
                        new GUIContent("阻塞", "是否会阻塞Feedbacks运行"),
                        value.Blocking);

                    value.RepeatForever = EditorGUILayout.Toggle(
                        new GUIContent("无限重复", "无限重复播放"),
                        value.RepeatForever);

                    value.AmountOfRepeat = EditorGUILayout.IntField(
                        new GUIContent("重复次数", "重复播放的次数"),
                        value.AmountOfRepeat);

                    value.IntervalBetweenRepeats = EditorGUILayout.FloatField(
                        new GUIContent("重复间隔", "每次循环播放的间隔"),
                        value.IntervalBetweenRepeats);
                }
            });

            DrawOtherPropertyLayout();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("播放"))
            {
                value.StartCoroutine(value.PlayCo());
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
