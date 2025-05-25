using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyFramework.Core;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [EditorSettingsAssetPath]
    [ShowOdinSerializedPropertiesInInspector]
    public class BinderSettings : ScriptableObjectSingleton<BinderSettings>, ISerializationCallbackReceiver
    {
        [HideLabel, InlineProperty, HideReferenceObjectPicker]
        public struct DefaultSettings
        {
            [LabelText("绑定游戏对象")]
            public bool BindGameObject;
            [LabelText("绑定权限")]
            public Binder.Access BindAccess;
            [LabelText("自动绑定名称")]
            public bool AutoBindName;
            [LabelText("处理绑定命名")]
            public bool ProcessBindName;
            [LabelText("使用文档注释")]
            public bool UseDocumentComment;

            [LabelText("自动添加注释段落")]
            [ShowIf(nameof(ShowAutoAddParaToComment))]
            public bool AutoAddParaToComment;

            [LabelText("注释")]
            public string Comment;

            private bool ShowAutoAddParaToComment => UseDocumentComment;
        }

        [TitleEx("默认值设置")]
        public DefaultSettings Default;

        [TitleEx("优先级设置")]
        [LabelText("优先级列表")]
        [ValueDropdown(nameof(GetTypesDropdown))]
        public Type[] PriorityTypes = new Type[]{};

        public int IndexByPriorityOf(Type type, bool checkDerive, bool checkBase)
        {
            int i = 0;
            foreach (var t in PriorityTypes)
            {
                if (t == type)
                    return i;

                i++;
            }

            i = 0;
            foreach (var t in PriorityTypes)
            {
                if (checkDerive)
                {
                    if (type.IsAssignableFrom(t))
                        return i;
                }

                if (checkBase)
                {
                    if (t.IsAssignableFrom(type))
                        return i;
                }

                i++;
            }

            return -1;
        }

        private static Type[] s_types;
        private IEnumerable GetTypesDropdown()
        {
            if (s_types.IsNullOrEmpty())
            {
                s_types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(asm => asm.GetTypes())
                    .Where(t => t.IsInterface || t.IsSubclassOf(typeof(Component)))
                    .ToArray();
            }
            return s_types;
        }

        [SerializeField, HideInInspector]
        private SerializationData _serializationData;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref _serializationData);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData);
        }
    }
}
