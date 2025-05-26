using Sirenix.OdinInspector.Editor;
using System;
using System.Linq;
using EasyFramework.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using EasyFramework.Core;

namespace EasyFramework.ToolKit.Editor
{
    [CustomEditor(typeof(Feedbacks))]
    [CanEditMultipleObjects]
    public class FeedbacksEditor : OdinEditor
    {
        private LocalPersistentContext<bool> _settingsExpandContext;
        private FoldoutGroupConfig _foldoutGroupConfig;

        private InspectorProperty _initializeModeProperty;
        private InspectorProperty _autoInitializationProperty;
        private InspectorProperty _autoPlayOnStartProperty;
        private InspectorProperty _autoPlayOnEnableProperty;
        private InspectorProperty _canPlayProperty;
        private InspectorProperty _canPlayWhileAlreadyPlayingProperty;
        private InspectorProperty _canMultiPlayProperty;
        private InspectorProperty _feedbacksProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _settingsExpandContext = this.GetPersistent("Settings/Expand", false);

            _initializeModeProperty = Tree.RootProperty.Children["_initializeMode"];
            _autoInitializationProperty = Tree.RootProperty.Children["_autoInitialization"];
            _autoPlayOnStartProperty = Tree.RootProperty.Children["_autoPlayOnStart"];
            _autoPlayOnEnableProperty = Tree.RootProperty.Children["_autoPlayOnEnable"];
            _canPlayProperty = Tree.RootProperty.Children["_canPlay"];
            _canPlayWhileAlreadyPlayingProperty = Tree.RootProperty.Children["_canPlayWhileAlreadyPlaying"];
            _canMultiPlayProperty = Tree.RootProperty.Children["_canMultiPlay"];
            _feedbacksProperty = Tree.RootProperty.Children["_feedbacks"];

            _foldoutGroupConfig = new FoldoutGroupConfig(
                this, new GUIContent("反馈设置"),
                _settingsExpandContext.Value,
                OnSettingsContentGUI);
        }

        private void OnSettingsContentGUI(Rect headerRect)
        {
            EasyEditorGUI.Title("初始化");

            _initializeModeProperty.DrawEx("初始化模式");
            _autoInitializationProperty.DrawEx("自动初始化", "确保播放前所有s都初始化Feedback");
            _autoPlayOnStartProperty.DrawEx("开始时自动播放", "在开始时自动播放一次");
            _autoPlayOnEnableProperty.DrawEx("启用时自动播放", "在启用时自动播放一次");

            EasyEditorGUI.Title("播放");
            
            _canPlayProperty.DrawEx("是否可以播放", "是否可以播放");
            _canPlayWhileAlreadyPlayingProperty.DrawEx("播放时是否可以继续播放", "在当前Play还没结束时是否可以开始新的播放");
            
            _canMultiPlayProperty.DrawEx("是否可以多重播放",
                "是否可以同时存在多个播放\n" +
                "注意：反馈的OnStop只会在最后一个播放结束时调用Feedback");
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            _foldoutGroupConfig.Expand = _settingsExpandContext.Value;
            _settingsExpandContext.Value = EasyEditorGUI.FoldoutGroup(_foldoutGroupConfig);

            // SirenixEditorGUI.BeginBox();

            _feedbacksProperty.DrawEx("反馈列表");

            var btnHeight = EditorGUIUtility.singleLineHeight;
            if (GUILayout.Button(
                    EditorHelper.TempContent("添加新的反馈...", image:EasyEditorIcons.AddDropdown),
                    GUILayout.Height(btnHeight)))
            {
                DoAddFeedback();
            }

            // SirenixEditorGUI.EndBox();

            EasyEditorGUI.Title("反馈调试");
            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            EditorGUILayout.BeginHorizontal();
            
            var feedbacksTargets = targets.Cast<Feedbacks>().ToArray();

            EditorGUI.BeginDisabledGroup(feedbacksTargets.All(feedbacks => feedbacks.IsInitialized));
            if (SirenixEditorGUI.SDFIconButton("初始化", btnHeight, SdfIconType.Tools))
            {
                feedbacksTargets.ForEach(feedbacks => feedbacks.Initialize());
            }

            EditorGUI.EndDisabledGroup();

            if (SirenixEditorGUI.SDFIconButton("播放", btnHeight, SdfIconType.Play))
            {
                feedbacksTargets.ForEach(feedbacks => feedbacks.Play());
            }

            if (SirenixEditorGUI.SDFIconButton("停止", btnHeight, SdfIconType.Stop))
            {
                feedbacksTargets.ForEach(feedbacks => feedbacks.Stop());
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            Tree.EndDraw();
        }

        private static Type[] s_allTypesFeedback;

        private static Type[] AllTypesFeedback
        {
            get
            {
                if (s_allTypesFeedback.IsNullOrEmpty())
                {
                    s_allTypesFeedback = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => t.HasInterface(typeof(IFeedback))
                                    && !t.IsAbstract && !t.IsInterface
                                    && t.HasCustomAttribute<AddFeedbackMenuAttribute>())
                        .ToArray();
                }

                return s_allTypesFeedback;
            }
        }

        private void DoAddFeedback()
        {
            var feedbacksTargets = targets.Cast<Feedbacks>().ToArray();

            void OnConfirm(Type t)
            {
                foreach (var feedbacks in feedbacksTargets)
                {
                    var inst = t.CreateInstance<IFeedback>();
                    if (feedbacks.IsInitialized)
                    {
                        inst.Initialize();
                    }

                    feedbacks.AddFeedback(inst);
                }
            }

            EasyEditorGUI.ShowSelectorInPopup(AllTypesFeedback, new PopupSelectorConfig(value => OnConfirm((Type)value))
            {
                MenuItemNameGetter = t => ((Type)t).GetCustomAttribute<AddFeedbackMenuAttribute>().Path,
                AddThumbnailIcons = false
            });
        }
    }
}
