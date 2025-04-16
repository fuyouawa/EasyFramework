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
    [CustomEditor(typeof(Feedbacks))]
    // [CanEditMultipleObjects]
    public class FeedbacksEditor : OdinEditor
    {
        private LocalPersistentContext<bool> _settingsExpandContext;
        private FoldoutGroupConfig _foldoutGroupConfig;

        private InspectorProperty _propertyOfInitializationMode;
        private InspectorProperty _propertyOfAutoInitialization;
        private InspectorProperty _propertyOfAutoPlayOnStart;
        private InspectorProperty _propertyOfAutoPlayOnEnable;
        private InspectorProperty _propertyOfCanPlay;
        private InspectorProperty _propertyOfCanPlayWhileAlreadyPlaying;
        private InspectorProperty _propertyOfCanMultiPlay;
        private InspectorProperty _propertyOfsFeedback;

        protected override void OnEnable()
        {
            base.OnEnable();
            _settingsExpandContext = this.GetPersistent("Settings/Expand", false);

            _propertyOfInitializationMode = Tree.RootProperty.Children["InitializationMode"];
            _propertyOfAutoInitialization = Tree.RootProperty.Children["AutoInitialization"];
            _propertyOfAutoPlayOnStart = Tree.RootProperty.Children["AutoPlayOnStart"];
            _propertyOfAutoPlayOnEnable = Tree.RootProperty.Children["AutoPlayOnEnable"];
            _propertyOfCanPlay = Tree.RootProperty.Children["CanPlay"];
            _propertyOfCanPlayWhileAlreadyPlaying = Tree.RootProperty.Children["CanPlayWhileAlreadyPlaying"];
            _propertyOfCanMultiPlay = Tree.RootProperty.Children["CanMultiPlay"];
            _propertyOfsFeedback = Tree.RootProperty.Children["_feedbacks"];

            _foldoutGroupConfig = new FoldoutGroupConfig(
                this, new GUIContent("反馈设置"),
                _settingsExpandContext.Value,
                OnSettingsContentGUI);
        }

        private void OnSettingsContentGUI(Rect headerRect)
        {
            EasyEditorGUI.Title("初始化");

            _propertyOfInitializationMode.DrawEx("初始化模式");
            _propertyOfAutoInitialization.DrawEx("自动初始化", "确保播放前所有s都初始化Feedback");
            _propertyOfAutoPlayOnStart.DrawEx("开始时自动播放", "在开始时自动播放一次");
            _propertyOfAutoPlayOnEnable.DrawEx("启用时自动播放", "在启用时自动播放一次");

            EasyEditorGUI.Title("播放");
            
            _propertyOfCanPlay.DrawEx("是否可以播放", "是否可以播放");
            _propertyOfCanPlayWhileAlreadyPlaying.DrawEx("播放时是否可以继续播放", "在当前Play还没结束时是否可以开始新的播放");
            
            _propertyOfCanMultiPlay.DrawEx("是否可以多重播放",
                "是否可以同时存在多个播放\n" +
                "注意：反馈的OnStop只会在最后一个播放结束时调用Feedback");
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            var feedbacks = (Feedbacks)target;

            _foldoutGroupConfig.Expand = _settingsExpandContext.Value;
            _settingsExpandContext.Value = EasyEditorGUI.FoldoutGroup(_foldoutGroupConfig);

            // SirenixEditorGUI.BeginBox();

            _propertyOfsFeedback.DrawEx("反馈列表");

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
            var feedbacks = (Feedbacks)target;

            void OnConfirm(Type t)
            {
                var inst = t.CreateInstance<IFeedback>();
                if (feedbacks.IsInitialized)
                {
                    inst.Initialize();
                }

                feedbacks.AddFeedback(inst);
            }

            EasyEditorGUI.ShowSelectorInPopup(AllTypesFeedback, new PopupSelectorConfig(value => OnConfirm((Type)value))
            {
                MenuItemNameGetter = t => ((Type)t).GetCustomAttribute<AddFeedbackMenuAttribute>().Path,
                AddThumbnailIcons = false
            });
        }
    }
}
