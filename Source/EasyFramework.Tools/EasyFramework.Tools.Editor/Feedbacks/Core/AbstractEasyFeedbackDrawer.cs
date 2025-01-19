using System;
using System.Linq;
using System.Reflection;
using EasyFramework.Generic;
using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
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

        private InspectorProperty _label;
        private InspectorProperty _enable;
        private InspectorProperty _delayBeforePlay;
        private InspectorProperty _blocking;
        private InspectorProperty _repeatForever;
        private InspectorProperty _amountOfRepeat;
        private InspectorProperty _intervalBetweenRepeats;


        protected override void Initialize()
        {
            base.Initialize();
            _label = Property.Children["Label"];
            _enable = Property.Children["Enable"];
            _delayBeforePlay = Property.Children["DelayBeforePlay"];
            _blocking = Property.Children["Blocking"];
            _repeatForever = Property.Children["RepeatForever"];
            _amountOfRepeat = Property.Children["AmountOfRepeat"];
            _intervalBetweenRepeats = Property.Children["IntervalBetweenRepeats"];
        }

        protected override void OnContentGUI(Rect headerRect)
        {
            var value = ValueEntry.SmartValue;

            if (value.Tip.IsNotNullOrWhiteSpace())
            {
                EasyEditorGUI.MessageBox(value.Tip, MessageType.Info);
            }

            _label.Draw(new GUIContent("标签"));
            _enable.Draw(new GUIContent("激活"));

            _label.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_label, this),
                "反馈设置",
                _label.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    _delayBeforePlay.Draw(new GUIContent("播放前延迟", "在正式Play前经过多少时间的延迟(s)"));
                    _blocking.Draw(new GUIContent("阻塞", "是否会阻塞反馈运行"));
                    _repeatForever.Draw(new GUIContent("无限重复", "无限重复播放"));

                    if (!value.RepeatForever)
                    {
                        _amountOfRepeat.Draw(new GUIContent("重复次数", "重复播放的次数"));
                    }

                    _intervalBetweenRepeats.Draw(new GUIContent("重复间隔", "每次循环播放的间隔"));
                }
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
