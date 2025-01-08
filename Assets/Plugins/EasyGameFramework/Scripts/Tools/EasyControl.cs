using System;
using EasyFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Serialization;
using Transform = UnityEngine.Transform;


#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

namespace EasyGameFramework
{
    [Serializable]
    public class EasyControlArgs
    {
        public bool Expand;
        public bool AsViewModel;
        public bool AsBinder;

        public bool IsViewModelInitialized;
        public bool IsBinderInitialized;

        public IEasyControl Parent;

        public string GenerateDir;
        public string ClassName;
    }

    public interface IEasyControl
    {
        public EasyControlArgs EasyControlArgs { get; }
    }

    public class EasyControl : MonoBehaviour, IEasyControl, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        private SerializationData _serializationData;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref this._serializationData);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref this._serializationData);
        }

        [SerializeField, HideInInspector]
        private EasyControlArgs _easyControlArgs = new();

        public EasyControlArgs EasyControlArgs => _easyControlArgs;
    }

#if UNITY_EDITOR
    public abstract class EasyControlEditorBase : OdinEditor
    {
        private List<Transform> _parents;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            var mono = (MonoBehaviour)target;
            _parents = mono.transform.FindParents(p =>
            {
                var c = p.GetComponent<IEasyControl>();
                if (c == null)
                    return false;
                return c.EasyControlArgs.AsViewModel;
            });
        }

        protected override void DrawTree()
        {
            base.DrawTree();

            var control = (IEasyControl)target;
            var mono = (MonoBehaviour)target;

            control.EasyControlArgs.Expand = EasyEditorGUI.FoldoutGroup(
                new EasyEditorGUI.FoldoutGroupConfig(this, "EasyControl设置", control.EasyControlArgs.Expand)
                {
                    OnContentGUI = rect =>
                    {
                        control.EasyControlArgs.AsViewModel = EditorGUILayout.Toggle("生成视图模型", control.EasyControlArgs.AsViewModel);

                        bool needExitGui = false;
                        if (control.EasyControlArgs.AsViewModel)
                        {
                            if (!control.EasyControlArgs.IsViewModelInitialized)
                            {
                                control.EasyControlArgs.ClassName = mono.gameObject.name;
                                control.EasyControlArgs.GenerateDir = "Scripts/Ui";

                                control.EasyControlArgs.IsViewModelInitialized = true;
                            }

                            EasyEditorGUI.BoxGroup(new EasyEditorGUI.BoxGroupConfig("视图模型配置")
                            {
                                OnContentGUI = headerRect =>
                                {
                                    EditorGUI.BeginChangeCheck();
                                    control.EasyControlArgs.GenerateDir = SirenixEditorFields.FolderPathField(new GUIContent("代码生成目录"), control.EasyControlArgs.GenerateDir, "Assets", false, false);
                                    needExitGui = EditorGUI.EndChangeCheck();
                                    control.EasyControlArgs.ClassName = EditorGUILayout.TextField("类名", control.EasyControlArgs.ClassName);
                                }
                            });
                        }

                        control.EasyControlArgs.AsBinder = EditorGUILayout.Toggle("作为被绑定者", control.EasyControlArgs.AsBinder);

                        if (control.EasyControlArgs.AsBinder)
                        {
                            if (!control.EasyControlArgs.IsBinderInitialized)
                            {
                                if (_parents.IsNotNullOrEmpty())
                                {
                                    control.EasyControlArgs.Parent = _parents[0].GetComponent<IEasyControl>()!;
                                    control.EasyControlArgs.IsBinderInitialized = true;
                                }
                            }

                            EasyEditorGUI.BoxGroup(new EasyEditorGUI.BoxGroupConfig("绑定配置")
                            {
                                OnContentGUI = headerRect =>
                                {
                                    var m = control.EasyControlArgs.Parent as MonoBehaviour;
                                    var lbl = m == null ? "None" : m.gameObject.name;
                                    EasyEditorGUI.DrawSelectorDropdown(
                                        new EasyEditorGUI.SelectorDropdownConfig<Transform>("父级", lbl, _parents,
                                            c => control.EasyControlArgs.Parent = c.GetComponent<IEasyControl>())
                                        {
                                            MenuItemNameGetter = c => c.gameObject.name
                                        });
                                }
                            });
                        }

                        if (SirenixEditorGUI.SDFIconButton("Update", EditorGUIUtility.singleLineHeight, SdfIconType.ArrowUpSquare))
                        {
                            
                        }

                        if (needExitGui)
                        {
                            GUIHelper.ExitGUI(false);
                        }
                    }
                });
        }
    }

    [CustomEditor(typeof(EasyControl))]
    public class EasyControlEditor : EasyControlEditorBase
    {
    }
#endif
}
