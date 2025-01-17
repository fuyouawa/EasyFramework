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
        private LocalPersistentContext<bool> _expand;

        protected override void OnEnable()
        {
            base.OnEnable();
            _expand = this.GetPersistent("_expand", true);
            var feedbacks = (EasyFeedbacks)target;
            
            feedbacks.FeedbackList.OnAddElementVoid = OnAddElement;
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);
            
            var feedbacks = (EasyFeedbacks)target;

            _expand.Value = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(this, "设置", _expand.Value)
            {
                OnContentGUI = rect =>
                {
                    EasyEditorGUI.Title("初始化");
                    feedbacks.InitializationMode = EnumSelector<EasyFeedbacks.InitializationModes>.DrawEnumField(
                        new GUIContent("初始化模式"),
                        feedbacks.InitializationMode);

                    feedbacks.AutoInitialization = EditorGUILayout.Toggle(
                        new GUIContent("自动初始化", "确保播放前所有Feedbacks都初始化"),
                        feedbacks.AutoInitialization);

                    feedbacks.AutoPlayOnStart = EditorGUILayout.Toggle(
                        new GUIContent("Start时自动播放", "在Start时自动播放一次"),
                        feedbacks.AutoPlayOnStart);

                    feedbacks.AutoPlayOnEnable = EditorGUILayout.Toggle(
                        new GUIContent("Enable时自动播放", "在OnEnable时自动播放一次"),
                        feedbacks.AutoPlayOnEnable);
                    
                    EasyEditorGUI.Title("播放");

                    feedbacks.CanPlay = EditorGUILayout.Toggle(
                        new GUIContent("是否可以播放", "是否可以播放"),
                        feedbacks.CanPlay);

                    feedbacks.CanPlayWhileAlreadyPlaying = EditorGUILayout.Toggle(
                        new GUIContent("播放时是否可以继续播放", "在当前Play还没结束时是否可以开始新的播放"),
                        feedbacks.CanPlayWhileAlreadyPlaying);

                    if (feedbacks.CanPlayWhileAlreadyPlaying)
                    {
                        feedbacks.CanMultiPlay = EditorGUILayout.Toggle(
                            new GUIContent("是否可以多重播放", "是否可以同时存在多个播放\n注意：Feedback的OnFeedbackStop只会在最后一个播放结束时调用"),
                            feedbacks.CanMultiPlay);
                    }
                }
            });
            Tree.RootProperty.Children["FeedbackList"].Draw(new GUIContent("反馈列表"));
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
