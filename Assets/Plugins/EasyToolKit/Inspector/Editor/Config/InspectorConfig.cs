using System;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Core.Editor.Internal;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    [EditorConfigsPath("Inspector")]
    public class InspectorConfig : ScriptableObjectSingleton<InspectorConfig>, ISerializationCallbackReceiver
    {
        private static bool s_hasUpdatedEditorsOnce = false;
        
        /// <summary>
        /// 检查当前是否为无图形界面
        /// </summary>
        private static bool IsHeadlessMode => SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;

        [SerializeField, HideInInspector] 
        private InspectorEditorTargets _defaultEditorTargets = InspectorEditorTargets.UserTypes | InspectorEditorTargets.PluginTypes | InspectorEditorTargets.OtherTypes;

        [SerializeField] 
        private bool _enableEasyInspector;

        // 存储自定义类型与其对应编辑器的配对信息
        [SerializeField]
        private List<TypeEditorPair> _configs = new List<TypeEditorPair>();

        // 运行时的类型到编辑器类型的快速查找缓存
        private Dictionary<Type, Type> _editorTypeCacheByDrawnType = new Dictionary<Type, Type>();

        
        public InspectorEditorTargets DefaultEditorTargets
        {
            get => _defaultEditorTargets;
            set => _defaultEditorTargets = value;
        }

        /// <summary>
        /// 更新自定义编辑器
        /// </summary>
        public void UpdateEditors()
        {
            // 在无图形界面模式或批处理模式下不执行
            if (IsHeadlessMode || InternalEditorUtility.inBatchMode)
                return;

            // 重置自定义编辑器
            CustomEditorUtility.ResetCustomEditors();

            // 如果启用了Easy检查器，则注册自定义编辑器
            if (_enableEasyInspector)
            {
                foreach (TypeEditorPair typeEditorPair in GetEditors())
                {
                    var drawnType = AssemblyUtility.GetTypeByFullName(typeEditorPair.DrawnTypeName);
                    var editorType = AssemblyUtility.GetTypeByFullName(typeEditorPair.EditorTypeName);

                    if (drawnType == null || editorType == null) continue;

                    CustomEditorUtility.SetCustomEditor(drawnType, editorType, isFallbackEditor: false, isEditorForChildClasses: false);
                }
            }

            EditorApplication.delayCall += () =>
            {
                Type inspectorWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
                Type activeEditorTrackerType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ActiveEditorTracker");

                if (inspectorWindowType != null && activeEditorTrackerType != null)
                {
                    var createTrackerMethod = inspectorWindowType.GetMethod("CreateTracker", BindingFlagsHelper.AllInstance());
                    var trackerField = inspectorWindowType.GetField("m_Tracker", BindingFlagsHelper.AllInstance());
                    var forceRebuild = activeEditorTrackerType.GetMethod("ForceRebuild", BindingFlagsHelper.AllInstance());

                    if (createTrackerMethod != null && trackerField != null && forceRebuild != null)
                    {
                        // 获取所有检查器窗口并强制重建
                        var windows = Resources.FindObjectsOfTypeAll(inspectorWindowType);

                        foreach (var window in windows)
                        {
                            createTrackerMethod.Invoke(window, null);
                            object tracker = trackerField.GetValue(window);
                            forceRebuild.Invoke(tracker, null);
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 更新并刷新检查器
        /// </summary>
        public void UpdateAndRefreshInspector()
        {
            this.UpdateEditors();
            // 保存当前选中的对象并重新选择它们以刷新检查器
            UnityEngine.Object[] objects = Selection.objects;
            Selection.objects = null;
            EditorApplication.delayCall +=(() => Selection.objects = objects);
        }

        /// <summary>
        /// 确保编辑器已经更新过至少一次
        /// </summary>
        internal void EnsureEditorsHaveBeenUpdated()
        {
            if (!s_hasUpdatedEditorsOnce)
            {
                this.UpdateEditors();
                s_hasUpdatedEditorsOnce = true;
            }
        }

        /// <summary>
        /// 序列化前回调
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        /// <summary>
        /// 序列化后回调，更新缓存
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UpdateCaches();
        }

        /// <summary>
        /// 重新构建内部类型到编辑器类型的缓存。
        /// </summary>
        public void UpdateCaches()
        {
            _editorTypeCacheByDrawnType.Clear();

            for (int i = 0; i < _configs.Count; i++)
            {
                var config = _configs[i];

                Type drawnType = TypeConverter.FromName(config.DrawnTypeName);

                if (drawnType == null)
                {
                    continue;
                }

                Type drawerType;

                if (string.IsNullOrEmpty(config.EditorTypeName))
                {
                    drawerType = null;
                }
                else
                {
                    drawerType = TypeConverter.FromName(config.EditorTypeName);

                    if (drawerType == null)
                    {
                        drawerType = typeof(MissingEditor);
                    }
                }

                _editorTypeCacheByDrawnType[drawnType] = drawerType;
            }
        }

        /// <summary>
        /// 获取所有已配置的编辑器及其绘制的类型。
        /// </summary>
        public List<TypeEditorPair> GetEditors()
        {
            var total = new List<TypeEditorPair>();

            foreach (var type in InspectorDrawingUtility.AllDrawnTypes)
            {
                var editor = GetEditorType(type);

                if (editor != null && editor != typeof(MissingEditor))
                {
                    total.Add(new TypeEditorPair(type, editor));
                }
            }

            return total;
        }

        /// <summary>
        /// 获取将绘制给定类型的编辑器类型。如果在配置中没有为该类型分配自定义编辑器类型，则返回默认编辑器类型。
        /// </summary>
        public Type GetEditorType(Type drawnType)
        {
            if (drawnType == null)
            {
                throw new ArgumentNullException(nameof(drawnType));
            }

            if (_editorTypeCacheByDrawnType.TryGetValue(drawnType, out Type editorType))
            {
                return editorType;
            }

            return InspectorDrawingUtility.GetDefaultEditorType(drawnType);
        }

        /// <summary>
        /// 分配一个给定的编辑器来绘制一个给定的类型。
        /// </summary>
        public void SetEditorType(Type drawnType, Type editorType)
        {
            if (drawnType == null)
            {
                throw new ArgumentNullException(nameof(drawnType));
            }

            string drawnTypeName = TypeConverter.ToName(drawnType);
            string editorTypeName = editorType == null ? string.Empty : TypeConverter.ToName(editorType);

            if (editorType != null)
            {
                if (!InspectorDrawingUtility.IsValidEditorBaseType(editorType, drawnType))
                {
                    throw new ArgumentException("The type " + editorType.GetNiceName() +
                                                " is not a valid base editor for type " + drawnType.GetNiceName() +
                                                ".");
                }
            }

            _editorTypeCacheByDrawnType[drawnType] = editorType;
            bool added = false;

            for (int i = 0; i < _configs.Count; i++)
            {
                var pair = _configs[i];

                if (pair.DrawnTypeName == drawnTypeName)
                {
                    pair.EditorTypeName = editorTypeName;
                    _configs[i] = pair;
                    added = true;
                    break;
                }
            }

            if (!added)
            {
                _configs.Add(new TypeEditorPair(drawnType, editorType));
            }

            EditorUtility.SetDirty(InspectorConfig.Instance);
        }

        /// <summary>
        /// 当配置的 editorTypeName 找不到时使用的占位类型。
        /// </summary>
        public static class MissingEditor { }
    }
}
