using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using EasyToolKit.Core.Internal.OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public static class CustomEditorUtility
    {
        public static readonly bool IsValid = CustomEditorUtility.UniversalAPI.IsValid &&
                                              (CustomEditorUtility.Unity_2023_1_API.IsValid ||
                                               CustomEditorUtility.Unity_Pre_2023_API.IsValid);

        static CustomEditorUtility()
        {
            if (CustomEditorUtility.IsValid)
                return;
            Debug.LogError(
                (object)("Unity's internal custom editor management classes have changed in this version of Unity (" +
                         Application.unityVersion +
                         "). Odin will not be able to dynamically register any editors; only hardcoded Odin editors will work."));
        }

        public static void ResetCustomEditors()
        {
            if (CustomEditorUtility.Unity_2023_1_API.IsValid)
            {
                CustomEditorUtility.Unity_2023_1_API.ResetCustomEditors();
            }
            else
            {
                if (!CustomEditorUtility.Unity_Pre_2023_API.IsValid)
                    return;
                CustomEditorUtility.Unity_Pre_2023_API.ResetCustomEditors();
            }
        }

        public static void SetCustomEditor(System.Type inspectedType, System.Type editorType)
        {
            if (!CustomEditorUtility.IsValid)
                return;
            CustomEditor customAttribute = CustomAttributeExtensions.GetCustomAttribute<CustomEditor>(editorType);
            if (customAttribute == null)
                throw new ArgumentException("Editor type to set '" + editorType.GetNiceName() +
                                            "' has no CustomEditor attribute applied! Use a SetCustomEditor overload that takes isFallbackEditor and isEditorForChildClasses parameters.");
            CustomEditorUtility.SetCustomEditor(inspectedType, editorType, customAttribute.isFallback,
                (bool)CustomEditorUtility.UniversalAPI.CustomEditor_EditorForChildClassesField.GetValue(
                    (object)customAttribute));
        }

        public static void SetCustomEditor(
            System.Type inspectedType,
            System.Type editorType,
            bool isFallbackEditor,
            bool isEditorForChildClasses)
        {
            if (!CustomEditorUtility.IsValid)
                return;
            CustomEditorUtility.SetCustomEditor(inspectedType, editorType, isFallbackEditor, isEditorForChildClasses,
                editorType.IsDefined<CanEditMultipleObjects>());
        }

        public static void SetCustomEditor(
            System.Type inspectedType,
            System.Type editorType,
            bool isFallbackEditor,
            bool isEditorForChildClasses,
            bool isMultiEditor)
        {
            if (!CustomEditorUtility.IsValid)
                return;
            object instance = Activator.CreateInstance(CustomEditorUtility.UniversalAPI.MonoEditorType);
            CustomEditorUtility.UniversalAPI.MonoEditorType_InspectorType.SetValue(instance, (object)editorType);
            CustomEditorUtility.UniversalAPI.MonoEditorType_IsFallback.SetValue(instance, (object)isFallbackEditor);
            CustomEditorUtility.UniversalAPI.MonoEditorType_EditorForChildClasses.SetValue(instance,
                (object)isEditorForChildClasses);
            if (CustomEditorUtility.Unity_2023_1_API.IsValid)
            {
                CustomEditorUtility.Unity_2023_1_API.RegisterCustomMonoEditorEntry(instance, inspectedType, editorType,
                    isMultiEditor);
            }
            else
            {
                if (!CustomEditorUtility.Unity_Pre_2023_API.IsValid)
                    return;
                CustomEditorUtility.Unity_Pre_2023_API.RegisterCustomMonoEditorEntry(instance, inspectedType,
                    editorType, isMultiEditor);
            }
        }

        private static class UniversalAPI
        {
            public static System.Type CustomEditorAttributesType;
            public static System.Type MonoEditorType;
            public static FieldInfo MonoEditorType_InspectorType;
            public static FieldInfo MonoEditorType_EditorForChildClasses;
            public static FieldInfo MonoEditorType_IsFallback;
            public static FieldInfo CustomEditor_EditorForChildClassesField;
            public static bool IsValid;

            static UniversalAPI()
            {
                try
                {
                    CustomEditorUtility.UniversalAPI.CustomEditorAttributesType =
                        typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.CustomEditorAttributes");
                    CustomEditorUtility.UniversalAPI.MonoEditorType =
                        CustomEditorUtility.UniversalAPI.CustomEditorAttributesType.GetNestedType(
                            nameof(MonoEditorType), BindingFlags.Public | BindingFlags.NonPublic);
                    FieldInfo field1 = CustomEditorUtility.UniversalAPI.MonoEditorType.GetField("m_InspectorType",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if ((object)field1 == null)
                        field1 = CustomEditorUtility.UniversalAPI.MonoEditorType.GetField("inspectorType",
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    CustomEditorUtility.UniversalAPI.MonoEditorType_InspectorType = field1;
                    FieldInfo field2 = CustomEditorUtility.UniversalAPI.MonoEditorType.GetField(
                        "m_EditorForChildClasses",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if ((object)field2 == null)
                        field2 = CustomEditorUtility.UniversalAPI.MonoEditorType.GetField("editorForChildClasses",
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    CustomEditorUtility.UniversalAPI.MonoEditorType_EditorForChildClasses = field2;
                    FieldInfo field3 = CustomEditorUtility.UniversalAPI.MonoEditorType.GetField("m_IsFallback",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if ((object)field3 == null)
                        field3 = CustomEditorUtility.UniversalAPI.MonoEditorType.GetField("isFallback",
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    CustomEditorUtility.UniversalAPI.MonoEditorType_IsFallback = field3;
                    CustomEditorUtility.UniversalAPI.CustomEditor_EditorForChildClassesField =
                        typeof(CustomEditor).GetField("m_EditorForChildClasses",
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    CustomEditorUtility.UniversalAPI.IsValid = true;
                }
                catch (NullReferenceException ex)
                {
                    CustomEditorUtility.UniversalAPI.IsValid = false;
                }

                if (!CustomEditorUtility.UniversalAPI.IsValid ||
                    !(CustomEditorUtility.UniversalAPI.MonoEditorType_InspectorType == (FieldInfo)null) &&
                    !(CustomEditorUtility.UniversalAPI.MonoEditorType_EditorForChildClasses == (FieldInfo)null) &&
                    !(CustomEditorUtility.UniversalAPI.MonoEditorType_IsFallback == (FieldInfo)null) &&
                    !(CustomEditorUtility.UniversalAPI.CustomEditor_EditorForChildClassesField == (FieldInfo)null))
                    return;
                CustomEditorUtility.UniversalAPI.IsValid = false;
            }
        }

        private static class Unity_2023_1_API
        {
            public static readonly PropertyInfo CustomEditorAttributesType_Instance;
            public static readonly MethodInfo CustomEditorAttributesType_Rebuild;
            public static readonly FieldInfo CustomEditorAttributesType_Cache;
            public static readonly System.Type CustomEditorCache_Type;
            public static readonly FieldInfo CustomEditorCache_CustomEditorCacheDict;
            public static readonly System.Type MonoEditorTypeStorage_Type;
            public static readonly FieldInfo MonoEditorTypeStorage_CustomEditors;
            public static readonly FieldInfo MonoEditorTypeStorage_CustomEditorsMultiEdition;
            public static readonly System.Type Dictionary_Type_MonoEditorTypeStorage;
            public static readonly MethodInfo Dictionary_Type_MonoEditorTypeStorage_Add;
            public static readonly MethodInfo Dictionary_Type_MonoEditorTypeStorage_TryGetValue;
            public static bool IsValid;

            static Unity_2023_1_API()
            {
                if (!CustomEditorUtility.UniversalAPI.IsValid)
                {
                    CustomEditorUtility.Unity_2023_1_API.IsValid = false;
                }
                else
                {
                    try
                    {
                        CustomEditorUtility.Unity_2023_1_API.CustomEditorAttributesType_Instance =
                            CustomEditorUtility.UniversalAPI.CustomEditorAttributesType.GetProperty("instance",
                                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        CustomEditorUtility.Unity_2023_1_API.CustomEditorAttributesType_Rebuild =
                            CustomEditorUtility.UniversalAPI.CustomEditorAttributesType.GetMethod("Rebuild",
                                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                BindingFlags.NonPublic, (Binder)null, System.Type.EmptyTypes,
                                (ParameterModifier[])null);
                        CustomEditorUtility.Unity_2023_1_API.CustomEditorAttributesType_Cache =
                            CustomEditorUtility.UniversalAPI.CustomEditorAttributesType.GetField("m_Cache",
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_Type =
                            CustomEditorUtility.UniversalAPI.CustomEditorAttributesType.GetNestedType(
                                "MonoEditorTypeStorage", BindingFlags.Public | BindingFlags.NonPublic);
                        CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_CustomEditors =
                            CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_Type.GetField("customEditors",
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_CustomEditorsMultiEdition =
                            CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_Type.GetField(
                                "customEditorsMultiEdition",
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        CustomEditorUtility.Unity_2023_1_API.CustomEditorCache_Type =
                            CustomEditorUtility.UniversalAPI.CustomEditorAttributesType.GetNestedType(
                                "CustomEditorCache", BindingFlags.Public | BindingFlags.NonPublic);
                        CustomEditorUtility.Unity_2023_1_API.CustomEditorCache_CustomEditorCacheDict =
                            CustomEditorUtility.Unity_2023_1_API.CustomEditorCache_Type.GetField("m_CustomEditorCache",
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        CustomEditorUtility.Unity_2023_1_API.Dictionary_Type_MonoEditorTypeStorage =
                            typeof(Dictionary<,>).MakeGenericType(typeof(System.Type),
                                CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_Type);
                        CustomEditorUtility.Unity_2023_1_API.Dictionary_Type_MonoEditorTypeStorage_Add =
                            CustomEditorUtility.Unity_2023_1_API.Dictionary_Type_MonoEditorTypeStorage.GetMethod("Add",
                                BindingFlags.Instance | BindingFlags.Public, (Binder)null, new System.Type[2]
                                {
                                    typeof(System.Type),
                                    CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_Type
                                }, (ParameterModifier[])null);
                        CustomEditorUtility.Unity_2023_1_API.Dictionary_Type_MonoEditorTypeStorage_TryGetValue =
                            CustomEditorUtility.Unity_2023_1_API.Dictionary_Type_MonoEditorTypeStorage.GetMethod(
                                "TryGetValue", BindingFlags.Instance | BindingFlags.Public, (Binder)null,
                                new System.Type[2]
                                {
                                    typeof(System.Type),
                                    CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_Type.MakeByRefType()
                                }, (ParameterModifier[])null);
                        if (CustomEditorUtility.Unity_2023_1_API.CustomEditorCache_CustomEditorCacheDict.FieldType !=
                            CustomEditorUtility.Unity_2023_1_API.Dictionary_Type_MonoEditorTypeStorage ||
                            CustomEditorUtility.Unity_2023_1_API.CustomEditorAttributesType_Rebuild ==
                            (MethodInfo)null ||
                            CustomEditorUtility.Unity_2023_1_API.CustomEditorAttributesType_Cache == (FieldInfo)null ||
                            CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_CustomEditors ==
                            (FieldInfo)null ||
                            CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_CustomEditorsMultiEdition ==
                            (FieldInfo)null ||
                            CustomEditorUtility.Unity_2023_1_API.CustomEditorCache_CustomEditorCacheDict ==
                            (FieldInfo)null ||
                            CustomEditorUtility.Unity_2023_1_API.Dictionary_Type_MonoEditorTypeStorage_Add ==
                            (MethodInfo)null ||
                            CustomEditorUtility.Unity_2023_1_API.Dictionary_Type_MonoEditorTypeStorage_TryGetValue ==
                            (MethodInfo)null)
                            return;
                        CustomEditorUtility.Unity_2023_1_API.IsValid = true;
                    }
                    catch (NullReferenceException ex)
                    {
                        CustomEditorUtility.Unity_2023_1_API.IsValid = false;
                    }
                }
            }

            public static void ResetCustomEditors()
            {
                if (!CustomEditorUtility.Unity_2023_1_API.IsValid)
                    return;
                if (CustomEditorUtility.Unity_2023_1_API.CustomEditorAttributesType_Rebuild.IsStatic)
                {
                    CustomEditorUtility.Unity_2023_1_API.CustomEditorAttributesType_Rebuild.Invoke((object)null,
                        (object[])null);
                }
                else
                {
                    object obj =
                        CustomEditorUtility.Unity_2023_1_API.CustomEditorAttributesType_Instance.GetValue((object)null);
                    CustomEditorUtility.Unity_2023_1_API.CustomEditorAttributesType_Rebuild.Invoke(obj, (object[])null);
                }
            }

            public static void RegisterCustomMonoEditorEntry(
                object entry,
                System.Type inspectedType,
                System.Type editorType,
                bool isMultiEditor)
            {
                if (!CustomEditorUtility.Unity_2023_1_API.IsValid)
                    return;
                object obj1 =
                    CustomEditorUtility.Unity_2023_1_API.CustomEditorAttributesType_Instance.GetValue((object)null);
                object obj2 = CustomEditorUtility.Unity_2023_1_API.CustomEditorAttributesType_Cache.GetValue(obj1);
                object obj3 =
                    CustomEditorUtility.Unity_2023_1_API.CustomEditorCache_CustomEditorCacheDict.GetValue(obj2);
                object[] parameters = new object[2]
                {
                    (object)inspectedType,
                    null
                };
                if (!(bool)CustomEditorUtility.Unity_2023_1_API.Dictionary_Type_MonoEditorTypeStorage_TryGetValue
                        .Invoke(obj3, parameters))
                {
                    parameters[1] =
                        Activator.CreateInstance(CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_Type);
                    CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_CustomEditors.SetValue(parameters[1],
                        Activator.CreateInstance(CustomEditorUtility.Unity_2023_1_API
                            .MonoEditorTypeStorage_CustomEditors.FieldType));
                    CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_CustomEditorsMultiEdition.SetValue(
                        parameters[1],
                        Activator.CreateInstance(CustomEditorUtility.Unity_2023_1_API
                            .MonoEditorTypeStorage_CustomEditorsMultiEdition.FieldType));
                    CustomEditorUtility.Unity_2023_1_API.Dictionary_Type_MonoEditorTypeStorage_Add.Invoke(obj3,
                        parameters);
                }

                object obj4 = parameters[1];
                (CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_CustomEditors.GetValue(obj4) as IList)
                    .Insert(0, entry);
                if (!isMultiEditor)
                    return;
                (CustomEditorUtility.Unity_2023_1_API.MonoEditorTypeStorage_CustomEditorsMultiEdition
                    .GetValue(obj4) as IList).Insert(0, entry);
            }
        }

        private static class Unity_Pre_2023_API
        {
            public static readonly FieldInfo CustomEditorAttributesType_CachedEditorForType;
            public static readonly FieldInfo CustomEditorAttributesType_CachedMultiEditorForType;
            public static readonly FieldInfo CustomEditorAttributesType_CustomEditors;
            public static readonly FieldInfo CustomEditorAttributesType_CustomMultiEditors;
            public static readonly FieldInfo CustomEditorAttributesType_Initialized;
            public static readonly MethodInfo CustomEditorAttributesType_Rebuild;
            public static FieldInfo MonoEditorType_InspectedType;
            public static readonly bool IsBackedByADictionary;
            public static bool IsValid;

            static Unity_Pre_2023_API()
            {
                if (!CustomEditorUtility.UniversalAPI.IsValid)
                {
                    CustomEditorUtility.Unity_Pre_2023_API.IsValid = false;
                }
                else
                {
                    try
                    {
                        CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_Initialized =
                            CustomEditorUtility.UniversalAPI.CustomEditorAttributesType.GetField("s_Initialized",
                                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CachedEditorForType =
                            CustomEditorUtility.UniversalAPI.CustomEditorAttributesType.GetField("kCachedEditorForType",
                                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CachedMultiEditorForType =
                            CustomEditorUtility.UniversalAPI.CustomEditorAttributesType.GetField(
                                "kCachedMultiEditorForType",
                                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CustomEditors =
                            CustomEditorUtility.UniversalAPI.CustomEditorAttributesType.GetField("kSCustomEditors",
                                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CustomMultiEditors =
                            CustomEditorUtility.UniversalAPI.CustomEditorAttributesType.GetField("kSCustomMultiEditors",
                                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_Rebuild =
                            CustomEditorUtility.UniversalAPI.CustomEditorAttributesType.GetMethod("Rebuild",
                                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        CustomEditorUtility.Unity_Pre_2023_API.MonoEditorType_InspectedType =
                            CustomEditorUtility.UniversalAPI.MonoEditorType.GetField("m_InspectedType",
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_Initialized ==
                            (FieldInfo)null ||
                            CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CustomEditors ==
                            (FieldInfo)null ||
                            CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CustomMultiEditors ==
                            (FieldInfo)null || CustomEditorUtility.Unity_Pre_2023_API.MonoEditorType_InspectedType ==
                            (FieldInfo)null)
                            throw new NullReferenceException();
                        CustomEditorUtility.Unity_Pre_2023_API.IsBackedByADictionary =
                            typeof(IDictionary).IsAssignableFrom(CustomEditorUtility.Unity_Pre_2023_API
                                .CustomEditorAttributesType_CustomEditors.FieldType);
                        CustomEditorUtility.Unity_Pre_2023_API.IsValid = true;
                    }
                    catch (NullReferenceException ex)
                    {
                        CustomEditorUtility.Unity_Pre_2023_API.IsValid = false;
                    }
                }
            }

            public static void ResetCustomEditors()
            {
                if (!CustomEditorUtility.Unity_Pre_2023_API.IsValid)
                    return;
                if (CustomEditorUtility.Unity_Pre_2023_API.IsBackedByADictionary)
                {
                    ((IDictionary)CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CustomEditors
                        .GetValue((object)null)).Clear();
                    ((IDictionary)CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CustomMultiEditors
                        .GetValue((object)null)).Clear();
                }
                else
                {
                    if (CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CachedEditorForType !=
                        (FieldInfo)null)
                        ((Dictionary<System.Type, System.Type>)CustomEditorUtility.Unity_Pre_2023_API
                            .CustomEditorAttributesType_CachedEditorForType.GetValue((object)null)).Clear();
                    if (CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CachedMultiEditorForType !=
                        (FieldInfo)null)
                        ((Dictionary<System.Type, System.Type>)CustomEditorUtility.Unity_Pre_2023_API
                            .CustomEditorAttributesType_CachedMultiEditorForType.GetValue((object)null)).Clear();
                    ((IList)CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CustomEditors.GetValue(
                        (object)null)).Clear();
                    ((IList)CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CustomMultiEditors
                        .GetValue((object)null)).Clear();
                }

                if (UnityVersion.IsVersionOrGreater(2019, 1))
                {
                    CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_Rebuild.Invoke((object)null,
                        (object[])null);
                    CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_Initialized.SetValue((object)null,
                        (object)true);
                }
                else
                    CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_Initialized.SetValue((object)null,
                        (object)false);
            }

            public static void RegisterCustomMonoEditorEntry(
                object entry,
                System.Type inspectedType,
                System.Type editorType,
                bool isMultiEditor)
            {
                if (!CustomEditorUtility.Unity_Pre_2023_API.IsValid)
                    return;
                CustomEditorUtility.Unity_Pre_2023_API.MonoEditorType_InspectedType.SetValue(entry,
                    (object)inspectedType);
                if (CustomEditorUtility.Unity_Pre_2023_API.IsBackedByADictionary)
                {
                    CustomEditorUtility.Unity_Pre_2023_API.AddEntryToDictList(
                        (IDictionary)CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CustomEditors
                            .GetValue((object)null), entry, inspectedType);
                    if (!isMultiEditor)
                        return;
                    CustomEditorUtility.Unity_Pre_2023_API.AddEntryToDictList(
                        (IDictionary)CustomEditorUtility.Unity_Pre_2023_API
                            .CustomEditorAttributesType_CustomMultiEditors.GetValue((object)null), entry,
                        inspectedType);
                }
                else
                {
                    if (CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CachedEditorForType !=
                        (FieldInfo)null &&
                        CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CachedMultiEditorForType !=
                        (FieldInfo)null)
                    {
                        ((IDictionary)CustomEditorUtility.Unity_Pre_2023_API
                            .CustomEditorAttributesType_CachedEditorForType
                            .GetValue((object)null))[(object)inspectedType] = (object)editorType;
                        if (isMultiEditor)
                            ((IDictionary)CustomEditorUtility.Unity_Pre_2023_API
                                .CustomEditorAttributesType_CachedMultiEditorForType
                                .GetValue((object)null))[(object)inspectedType] = (object)editorType;
                    }

                    ((IList)CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CustomEditors.GetValue(
                        (object)null)).Insert(0, entry);
                    if (!isMultiEditor)
                        return;
                    ((IList)CustomEditorUtility.Unity_Pre_2023_API.CustomEditorAttributesType_CustomMultiEditors
                        .GetValue((object)null)).Insert(0, entry);
                }
            }

            private static void AddEntryToDictList(IDictionary dict, object entry, System.Type inspectedType)
            {
                IList instance;
                if (dict.Contains((object)inspectedType))
                {
                    instance = (IList)dict[(object)inspectedType];
                }
                else
                {
                    instance = (IList)Activator.CreateInstance(
                        typeof(List<>).MakeGenericType(CustomEditorUtility.UniversalAPI.MonoEditorType));
                    dict[(object)inspectedType] = (object)instance;
                }

                instance.Insert(0, entry);
            }
        }
    }
}
