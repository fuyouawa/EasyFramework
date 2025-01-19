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
        private InspectorProperty _initializationMode;
        private InspectorProperty _autoInitialization;
        private InspectorProperty _autoPlayOnStart;
        private InspectorProperty _autoPlayOnEnable;
        private InspectorProperty _canPlay;
        private InspectorProperty _canPlayWhileAlreadyPlaying;
        private InspectorProperty _canMultiPlay;
        private InspectorProperty _feedbackList;

        protected override void OnEnable()
        {
            base.OnEnable();
            var feedbacks = (EasyFeedbacks)target;
            feedbacks.FeedbackList.OnAddElementVoid = OnAddElement;

            _initializationMode = Tree.RootProperty.Children["InitializationMode"];
            _autoInitialization = Tree.RootProperty.Children["AutoInitialization"];
            _autoPlayOnStart = Tree.RootProperty.Children["AutoPlayOnStart"];
            _autoPlayOnEnable = Tree.RootProperty.Children["AutoPlayOnEnable"];
            _canPlay = Tree.RootProperty.Children["CanPlay"];
            _canPlayWhileAlreadyPlaying = Tree.RootProperty.Children["CanPlayWhileAlreadyPlaying"];
            _canMultiPlay = Tree.RootProperty.Children["CanMultiPlay"];
            _feedbackList = Tree.RootProperty.Children["FeedbackList"];
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);
            
            var feedbacks = (EasyFeedbacks)target;

            _initializationMode.State.Expanded = EasyEditorGUI.FoldoutGroup(
                new FoldoutGroupConfig(this, "设置", _initializationMode.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    EasyEditorGUI.Title("初始化");

                    _initializationMode.Draw(new GUIContent("初始化模式"));
                    _autoInitialization.Draw(new GUIContent("自动初始化", "确保播放前所有Feedbacks都初始化"));
                    _autoPlayOnStart.Draw(new GUIContent("开始时自动播放", "在开始时自动播放一次"));
                    _autoPlayOnEnable.Draw(new GUIContent("激活时自动播放", "在激活时自动播放一次"));

                    EasyEditorGUI.Title("播放");
                    _canPlay.Draw(new GUIContent("是否可以播放", "是否可以播放"));
                    _canPlayWhileAlreadyPlaying.Draw(new GUIContent("播放时是否可以继续播放", "在当前Play还没结束时是否可以开始新的播放"));

                    if (feedbacks.CanPlayWhileAlreadyPlaying)
                    {
                        _canMultiPlay.Draw(new GUIContent(
                            "是否可以多重播放",
                            "是否可以同时存在多个播放\n" +
                            "注意：反馈的OnFeedbackStop只会在最后一个播放结束时调用"));
                    }
                }
            });
            _feedbackList.Draw(new GUIContent("反馈列表"));

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
