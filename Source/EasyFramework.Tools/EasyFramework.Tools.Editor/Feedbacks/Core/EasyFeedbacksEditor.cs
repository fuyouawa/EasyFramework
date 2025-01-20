using Sirenix.OdinInspector.Editor;
using System;
using System.Linq;
using EasyFramework.Generic;
using EasyFramework.Inspector;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [CustomEditor(typeof(EasyFeedbacks))]
    [CanEditMultipleObjects]
    public class EasyFeedbacksEditor : OdinEditor
    {
        private InspectorProperty _feedbackListProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            var feedbacks = (EasyFeedbacks)target;
            feedbacks.FeedbackList.OnAddElementVoid = OnAddElement;

            _feedbackListProperty = Tree.RootProperty.Children[nameof(EasyFeedbacks.FeedbackList)];
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            var feedbacks = (EasyFeedbacks)target;

            _feedbackListProperty.State.Expanded = EasyEditorGUI.FoldoutGroup(
                new FoldoutGroupConfig(this, "设置", _feedbackListProperty.State.Expanded)
                {
                    OnContentGUI = rect =>
                    {
                        EasyEditorGUI.Title("初始化");

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
                });

            _feedbackListProperty.Draw(EditorHelper.TempContent("反馈列表"));

            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(feedbacks.IsInitialized);
            if (GUILayout.Button("初始化"))
            {
                feedbacks.Initialize();
            }

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("播放"))
            {
                feedbacks.Play();
            }

            if (GUILayout.Button("停止"))
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

        private void OnAddElement()
        {
            var feedbacks = (EasyFeedbacks)target;

            void OnConfirm(Type t)
            {
                var inst = t.CreateInstance<AbstractEasyFeedback>();
                if (feedbacks.IsInitialized)
                {
                    inst.Initialize();
                }

                feedbacks.FeedbackList.Add(inst);
            }

            EasyEditorGUI.ShowSelectorInPopup(new PopupSelectorConfig<Type>(AllFeedbackTypes, OnConfirm)
            {
                MenuItemNameGetter = t => t.GetCustomAttribute<AddEasyFeedbackMenuAttribute>().Path,
                AddThumbnailIcons = false
            });
        }
    }
}
