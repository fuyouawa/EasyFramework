using System;
using System.Collections.Generic;
using System.Linq;
using EasyFramework.Editor.Drawer;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyFramework.Editor
{
    [EditorSettingsAssetPath]
    public class ViewBinderSettings : ScriptableObjectSingleton<ViewBinderSettings>
    {
        [Serializable, HideLabel, InlineProperty, HideReferenceObjectPicker]
        public class PriorityList : ISerializationCallbackReceiver
        {
            [ShowInInspector, NonSerialized]
            [LabelText("优先级")]
            [TypeSelectorSettings(FilterTypesFunction = "TypeFilter")]
            public List<Type> Collection = new List<Type>();

            [HideInInspector, SerializeField]
            private List<string> _serializedCollection = new List<string>();

            public int IndexOf(Type type, bool checkDerive, bool checkBase)
            {
                int i = 0;
                foreach (var t in Collection)
                {
                    if (t == type)
                    {
                        return i;
                    }

                    i++;
                }

                i = 0;
                foreach (var t in Collection)
                {
                    if (checkDerive)
                    {
                        if (type.IsAssignableFrom(t))
                        {
                            return i;
                        }
                    }

                    if (checkBase)
                    {
                        if (type.IsSubclassOf(t))
                        {
                            return i;
                        }
                    }

                    i++;
                }

                return -1;
            }

            public void OnBeforeSerialize()
            {
                _serializedCollection.Clear();

                foreach (var type in Collection)
                {
                    _serializedCollection.Add(TwoWaySerializationBinder.Default.BindToName(type));
                }
            }

            public void OnAfterDeserialize()
            {
                if (Collection.IsNotNullOrEmpty())
                    return;

                foreach (var type in _serializedCollection)
                {
                    Collection.Add(TwoWaySerializationBinder.Default.BindToType(type));
                }
            }

            private static HashSet<Type> s_types;

            private bool TypeFilter(Type type)
            {
                if (s_types.IsNullOrEmpty())
                {
                    s_types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => t.IsSubclassOf(typeof(Component)) && t.IsPublic)
                        .ToHashSet();
                }

                return s_types.Contains(type);
            }
        }

        [LabelText("自动命名空间")] public bool AutoNamingNotations = true;

        [LabelText("注释")] public string Comment;

        [LabelText("访问标识符")] public ViewBindAccess Access;

        [LabelText("注释自动添加段落xml")] public bool AutoAddCommentPara = true;

        public PriorityList Priority;
    }
}
