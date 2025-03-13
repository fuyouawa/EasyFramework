using Sirenix.OdinInspector.Editor;
using System;
using System.Linq;
using System.Reflection;
using EasyFramework.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [CustomEditor(typeof(EasyFeedbacks))]
    // [CanEditMultipleObjects]
    public class EasyFeedbacksEditor : OdinEditor
    {
        private InspectorProperty _feedbackListProperty;
        private LocalPersistentContext<bool> _settingsExpandContext;

        private FoldoutGroupConfig _foldoutGroupConfig;

        protected override void OnEnable()
        {
            base.OnEnable();
            _feedbackListProperty = Tree.RootProperty.Children["_feedbacks"];
            _settingsExpandContext = this.GetPersistent("Settings/Expand", false);

            _foldoutGroupConfig = new FoldoutGroupConfig(
                this, new GUIContent("反馈设置"),
                _settingsExpandContext.Value,
                OnSettingsContentGUI);
        }

        private void OnSettingsContentGUI(Rect headerRect)
        {
            EasyEditorGUI.Title("初始化");

            var feedbacks = (EasyFeedbacks)target;

            feedbacks.InitializationMode = EasyEditorField.Enum(
                EditorHelper.TempContent("初始化模式"),
                feedbacks.InitializationMode);
            feedbacks.AutoInitialization = EasyEditorField.Value(
                EditorHelper.TempContent("自动初始化", "确保播放前所有Feedbacks都初始化"),
                feedbacks.AutoInitialization);
            feedbacks.AutoPlayOnStart = EasyEditorField.Value(
                EditorHelper.TempContent("开始时自动播放", "在开始时自动播放一次"),
                feedbacks.AutoPlayOnStart);
            feedbacks.AutoPlayOnEnable = EasyEditorField.Value(
                EditorHelper.TempContent("启用时自动播放", "在启用时自动播放一次"),
                feedbacks.AutoPlayOnEnable);

            EasyEditorGUI.Title("播放");

            feedbacks.CanPlay = EasyEditorField.Value(
                EditorHelper.TempContent("是否可以播放", "是否可以播放"),
                feedbacks.CanPlay);
            feedbacks.CanPlayWhileAlreadyPlaying = EasyEditorField.Value(
                EditorHelper.TempContent("播放时是否可以继续播放", "在当前Play还没结束时是否可以开始新的播放"),
                feedbacks.CanPlayWhileAlreadyPlaying);

            if (feedbacks.CanPlayWhileAlreadyPlaying)
            {
                feedbacks.CanMultiPlay = EasyEditorField.Value(
                    EditorHelper.TempContent("是否可以多重播放",
                        "是否可以同时存在多个播放\n" +
                        "注意：反馈的OnFeedbackStop只会在最后一个播放结束时调用"),
                    feedbacks.CanMultiPlay);
            }
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            var feedbacks = (EasyFeedbacks)target;

            _foldoutGroupConfig.Expand = _settingsExpandContext.Value;
            _settingsExpandContext.Value = EasyEditorGUI.FoldoutGroup(_foldoutGroupConfig);

            // SirenixEditorGUI.BeginBox();

            _feedbackListProperty.Draw(EditorHelper.TempContent("反馈列表"));

            var btnHeight = EditorGUIUtility.singleLineHeight;
            if (GUILayout.Button(EditorHelper.TempContent("添加新的反馈...", image:EasyEditorIcons.AddDropdown)))
            {
                DoAddFeedback();
            }

            // SirenixEditorGUI.EndBox();

            EasyEditorGUI.Title("反馈调试");
            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(feedbacks.IsInitialized);
            if (SirenixEditorGUI.SDFIconButton("初始化", btnHeight, SdfIconType.Tools))
            {
                feedbacks.Initialize();
            }

            EditorGUI.EndDisabledGroup();

            if (SirenixEditorGUI.SDFIconButton("播放", btnHeight, SdfIconType.Play))
            {
                feedbacks.Play();
            }

            if (SirenixEditorGUI.SDFIconButton("停止", btnHeight, SdfIconType.Stop))
            {
                feedbacks.Stop();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            Tree.EndDraw();
        }

        private static Type[] s_allFeedbackTypes;

        private static Type[] AllFeedbackTypes
        {
            get
            {
                if (s_allFeedbackTypes == null)
                {
                    s_allFeedbackTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => t.IsSubclassOf(typeof(AbstractEasyFeedback))
                                    && !t.IsAbstract
                                    && t.HasCustomAttribute<AddEasyFeedbackMenuAttribute>())
                        .ToArray();
                }

                return s_allFeedbackTypes;
            }
        }

        private void DoAddFeedback()
        {
            var feedbacks = (EasyFeedbacks)target;

            void OnConfirm(Type t)
            {
                var inst = t.CreateInstance<AbstractEasyFeedback>();
                if (feedbacks.IsInitialized)
                {
                    inst.Initialize();
                }

                feedbacks.Feedbacks.Add(inst);
            }

            EasyEditorGUI.ShowSelectorInPopup(AllFeedbackTypes, new PopupSelectorConfig(value => OnConfirm((Type)value))
            {
                MenuItemNameGetter = t => ((Type)t).GetCustomAttribute<AddEasyFeedbackMenuAttribute>().Path,
                AddThumbnailIcons = false
            });
        }
    }
}
