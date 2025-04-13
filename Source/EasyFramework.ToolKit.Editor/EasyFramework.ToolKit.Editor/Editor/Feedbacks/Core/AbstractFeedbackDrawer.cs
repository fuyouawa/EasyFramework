using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    internal class EasyHelperFeedback
    {
        private static FieldInfo[] s_ignoreDrawFields;
        public static FieldInfo[] IgnoreDrawFields => s_ignoreDrawFields ??= typeof(AbstractFeedback).GetFields().ToArray();
    }

    internal class PropertiesGroupInfo
    {
        public GUIContent Label = new GUIContent();
        public OnContentGUIDelegate OnContentGUI;

        public PropertiesGroupInfo(string label, OnContentGUIDelegate onContentGUI)
        {
            Label.text = label;
            OnContentGUI = onContentGUI;
        }

        public PropertiesGroupInfo(GUIContent label, OnContentGUIDelegate onContentGUI)
        {
            Label = label;
            OnContentGUI = onContentGUI;
        }
    }

    internal class FoldoutState
    {
        public bool Expanded = true;
    }


    public class AbstractFeedbackDrawer<T> : FoldoutValueDrawer<T>
        where T : AbstractFeedback
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
        private readonly List<LocalPersistentContext<FoldoutState>> _foldoutStates = new List<LocalPersistentContext<FoldoutState>>();
        private readonly List<FoldoutGroupConfig> _foldoutGroupConfigs = new List<FoldoutGroupConfig>();
        private bool _autoDrawOtherProperties = false;

        internal List<PropertiesGroupInfo> PropertiesGroups = new List<PropertiesGroupInfo>();

        private InspectorProperty _propertyOfLabel;
        private InspectorProperty _propertyOfEnable;
        private InspectorProperty _propertyOfDelayBeforePlay;
        private InspectorProperty _propertyOfBlocking;
        private InspectorProperty _propertyOfRepeatForever;
        private InspectorProperty _propertyOfAmountOfRepeat;
        private InspectorProperty _propertyOfIntervalBetweenRepeats;

        protected T Feedback { get; private set; }

        protected override void Initialize()
        {
            base.Initialize();
            Feedback = ValueEntry.SmartValue;

            PropertiesGroups.Add(new PropertiesGroupInfo("反馈设置", OnSettingsContentGUI));
            PostBuildPropertiesGroups();

            for (int i = 0; i < PropertiesGroups.Count; i++)
            {
                var info = PropertiesGroups[i];
                var key = $"FoldoutState/{i}";
                var state = Property.Context.GetPersistent(this, key, new FoldoutState());
                _foldoutStates.Add(state);

                _foldoutGroupConfigs.Add(new FoldoutGroupConfig(
                    key, info.Label, true, info.OnContentGUI));
            }

            var style = new GUIStyle(SirenixGUIStyles.BoldLabel);
            style.fontSize += 1;
            style.padding.bottom += 2;
            style.padding.left -= 9;
            _labelConfig.Style = style;

            _propertyOfLabel = Property.Children["Label"];
            _propertyOfEnable = Property.Children["Enable"];
            _propertyOfDelayBeforePlay = Property.Children["DelayBeforePlay"];
            _propertyOfBlocking = Property.Children["Blocking"];
            _propertyOfRepeatForever = Property.Children["RepeatForever"];
            _propertyOfAmountOfRepeat = Property.Children["AmountOfRepeat"];
            _propertyOfIntervalBetweenRepeats = Property.Children["IntervalBetweenRepeats"];
        }

        protected virtual void PostBuildPropertiesGroups()
        {
            _autoDrawOtherProperties = true;
        }

        private void OnSettingsContentGUI(Rect headerRect)
        {
            _propertyOfDelayBeforePlay.DrawEx("播放前延迟", "在正式Play前经过多少时间的延迟(s)");
            _propertyOfBlocking.DrawEx("阻塞", "是否会阻塞反馈运行");
            _propertyOfRepeatForever.DrawEx("无限重复", "无限重复播放");
            
            _propertyOfAmountOfRepeat.DrawEx("重复次数", "重复播放的次数");
            _propertyOfIntervalBetweenRepeats.DrawEx("重复间隔", "每次循环播放的间隔");
        }

        protected override void OnContentGUI(Rect headerRect)
        {
            if (Feedback.Tip.IsNotNullOrWhiteSpace())
            {
                EasyEditorGUI.MessageBox(Feedback.Tip, MessageType.Info);
            }
            
            _propertyOfLabel.DrawEx("标签");
            _propertyOfEnable.DrawEx("启用");

            DrawPropertiesGroups();
            if (_autoDrawOtherProperties)
            {
                DrawOtherProperties();
            }

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

        private void DrawOtherProperties()
        {
            foreach (var child in Property.Children)
            {
                if (!Array.Exists(EasyHelperFeedback.IgnoreDrawFields, f => f.Name == child.Name))
                {
                    child.Draw();
                }
            }
        }

        private void DrawPropertiesGroups()
        {
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < PropertiesGroups.Count; i++)
            {
                FoldoutGroup(_foldoutGroupConfigs[i], _foldoutStates[i].Value);
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(GetTargetComponent());
            }

            return;

            void FoldoutGroup(FoldoutGroupConfig config, FoldoutState state)
            {
                config.Expand = state.Expanded;
                state.Expanded = EasyEditorGUI.FoldoutGroup(config);
            }
        }

        private Component GetTargetComponent()
        {
            return Property.Parent.Parent.WeakSmartValue<Component>();
        }
    }
}
