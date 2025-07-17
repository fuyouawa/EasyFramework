// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using System.Runtime.CompilerServices;
// using EasyToolKit.Core;
// using EasyToolKit.Core.Editor;
// using UnityEditor;
// using UnityEngine;
//
// namespace EasyToolKit.Inspector.Editor
// {
//     internal static class InspectorDrawingUtility
//     {
//         private static readonly FieldInfo InspectedTypeField =
//             typeof(CustomEditor).GetField("m_InspectedType", BindingFlagsHelper.AllInstance());
//
//         private static readonly FieldInfo EditorForChildClassesField =
//             typeof(CustomEditor).GetField("m_EditorForChildClasses", BindingFlagsHelper.AllInstance());
//
//         internal static readonly HashSet<Type> EligibleDrawnTypes = new HashSet<Type>();
//         internal static readonly HashSet<Type> AllDrawnTypes = new HashSet<Type>();
//
//         static InspectorDrawingUtility()
//         {
//             if (InspectedTypeField == null || EditorForChildClassesField == null)
//             {
//                 Debug.LogWarning(
//                     "Could not find internal fields 'm_InspectedType' and/or 'm_EditorForChildClasses' in type UnityEditor.CustomEditor. Automatic inspector editor generation is highly unlikely to work.");
//             }
//
//             // 初始化不应绘制自定义编辑器的类型集合
//             var neverDrawTypes = new HashSet<Type>();
//             var networkView = AssemblyUtility.GetTypeByFullName("UnityEngine.NetworkView");
//             if (networkView != null) neverDrawTypes.Add(networkView);
//             var guiText = AssemblyUtility.GetTypeByFullName("UnityEngine.GUIText");
//             if (guiText != null) neverDrawTypes.Add(guiText);
//
//             // 获取所有继承自 UnityEngine.Object 的类型
//             TypeCache.TypeCollection typesDerivedFrom = TypeCache.GetTypesDerivedFrom(typeof(UnityEngine.Object));
//             var unityObjectTypes = new List<Type>(typesDerivedFrom.Count);
//             using (TypeCache.TypeCollection.Enumerator enumerator = typesDerivedFrom.GetEnumerator())
//             {
//                 while (enumerator.MoveNext())
//                 {
//                     Type current = enumerator.Current;
//                     // 过滤掉编译器生成的、已过时的、不应绘制的以及 Joint 的子类
//                     if (!current.IsDefined(typeof(CompilerGeneratedAttribute), false) &&
//                         !current.IsDefined(typeof(ObsoleteAttribute), false) && !neverDrawTypes.Contains(current) &&
//                         !Type.GetType("UnityEngine.Joint, UnityEngine.PhysicsModule").IsAssignableFrom(current))
//                         unityObjectTypes.Add(current);
//                 }
//             }
//
//             // 存储已有绘制器的类型及其编辑器
//             Dictionary<Type, Type> haveDrawersAlready = new Dictionary<Type, Type>();
//             // 存储为派生类绘制的类型及其编辑器
//             Dictionary<Type, Type> derivedClassDrawnTypes = new Dictionary<Type, Type>();
//
//             // 遍历所有 UnityObject 类型，查找编辑器类型
//             foreach (var type in unityObjectTypes)
//             {
//                 if (typeof(UnityEditor.Editor).IsAssignableFrom(type))
//                 {
//                     try
//                     {
//                         var drawnType = GetEditorDrawnType(type, out bool editorForChildClasses);
//
//                         if (drawnType != null)
//                         {
//                             if (!haveDrawersAlready.ContainsKey(drawnType))
//                             {
//                                 haveDrawersAlready.Add(drawnType, type);
//                             }
//
//                             if (editorForChildClasses && !derivedClassDrawnTypes.ContainsKey(drawnType))
//                             {
//                                 derivedClassDrawnTypes.Add(drawnType, type);
//                             }
//                         }
//                     }
//                     catch (TypeLoadException)
//                     {
//                         // 忽略类型加载异常
//                     }
//                     catch (ReflectionTypeLoadException)
//                     {
//                         // 忽略反射类型加载异常
//                     }
//                 }
//             }
//
//             // 定义查找基类时的停止类型
//             HashSet<Type> stopBaseTypeLookUpTypes = new HashSet<Type>()
//             {
//                 typeof(object),
//                 typeof(Component),
//                 typeof(Behaviour),
//                 typeof(MonoBehaviour),
//                 typeof(UnityEngine.Object),
//                 typeof(ScriptableObject),
//                 Type.GetType("UnityEngine.StateMachineBehaviour, UnityEngine.AnimationModule")
//             };
//
//             // 遍历所有非编辑器、非抽象、非泛型的 UnityObject 类型
//             foreach (var type in unityObjectTypes
//                          .Where(type =>
//                              !type.IsAbstract && !type.IsGenericTypeDefinition && !type.IsGenericType &&
//                              !typeof(UnityEditor.Editor).IsAssignableFrom(type) &&
//                              !typeof(EditorWindow).IsAssignableFrom(type)))
//             {
//                 bool haveDrawerAlready = haveDrawersAlready.TryGetValue(type, out Type preExistingEditorType);
//
//                 // 如果没有直接的绘制器，则向上查找基类的绘制器
//                 if (!haveDrawerAlready)
//                 {
//                     Type baseType = type.BaseType;
//
//                     while (baseType != null && !stopBaseTypeLookUpTypes.Contains(baseType))
//                     {
//                         if (derivedClassDrawnTypes.TryGetValue(baseType, out Type editor))
//                         {
//                             haveDrawerAlready = true;
//                             preExistingEditorType = editor;
//                             break;
//                         }
//
//                         baseType = baseType.BaseType;
//                     }
//                 }
//
//                 // 如果该类型没有预先存在的编辑器，则将其添加到候选绘制类型列表中
//                 if (!haveDrawerAlready)
//                 {
//                     EligibleDrawnTypes.Add(type);
//                 }
//
//                 AllDrawnTypes.Add(type);
//             }
//         }
//
//         /// <summary>
//         /// 确定是否可以为给定类型创建自定义编辑器。
//         /// </summary>
//         public static bool CanCreateCustomEditorFor(Type type)
//         {
//             return EligibleDrawnTypes.Contains(type);
//         }
//
//         public static Type GetDefaultEditorType(Type drawnType)
//         {
//             if (drawnType == null)
//                 throw new ArgumentNullException(nameof(drawnType));
//             if (!CanCreateCustomEditorFor(drawnType))
//                 return null;
//         
//             if (InspectorConfig.Instance.DefaultEditorTargets == InspectorEditorTargets.None)
//                 return null;
//         
//             bool useEasyInspector;
//             switch (AssemblyUtility.GetAssemblyCategory(drawnType.Assembly))
//             {
//                 case AssemblyCategory.Scripts:
//                     useEasyInspector =
//                         (InspectorConfig.Instance.DefaultEditorTargets & InspectorEditorTargets.UserTypes) ==
//                         InspectorEditorTargets.UserTypes;
//                     break;
//                 case AssemblyCategory.ImportedAssemblies:
//                     useEasyInspector =
//                         (InspectorConfig.Instance.DefaultEditorTargets & InspectorEditorTargets.PluginTypes) ==
//                         InspectorEditorTargets.PluginTypes;
//                     break;
//                 case AssemblyCategory.UnityEngine:
//                     useEasyInspector =
//                         (InspectorConfig.Instance.DefaultEditorTargets & InspectorEditorTargets.UnityTypes) ==
//                         InspectorEditorTargets.UnityTypes;
//                     break;
//                 default:
//                     useEasyInspector =
//                         (InspectorConfig.Instance.DefaultEditorTargets & InspectorEditorTargets.OtherTypes) ==
//                         InspectorEditorTargets.OtherTypes;
//                     break;
//             }
//         
//             if (useEasyInspector)
//                 return typeof(EasyEditor);
//             return null;
//         }
//
//         /// <summary>
//         /// 检查给定的编辑器是否可以分配用于绘制给定的类型。
//         /// </summary>
//         public static bool IsValidEditorBaseType(Type editorType, Type drawnType)
//         {
//             if (editorType == null)
//             {
//                 throw new ArgumentNullException(nameof(editorType));
//             }
//
//             if (editorType.IsAbstract || !typeof(UnityEditor.Editor).IsAssignableFrom(editorType) ||
//                 editorType.FullName.StartsWith("UnityEditor", StringComparison.InvariantCulture))
//             {
//                 return false;
//             }
//
//             if (InspectedTypeField == null)
//             {
//                 return false;
//             }
//
//             var attribute = editorType.GetCustomAttribute<CustomEditor>(true);
//
//             if (attribute == null)
//             {
//                 return true;
//             }
//
//             if (drawnType != null)
//             {
//                 Type inspectedType = (Type)InspectedTypeField.GetValue(attribute);
//
//                 if (inspectedType == drawnType)
//                 {
//                     return true;
//                 }
//                 else if (EditorForChildClassesField != null && inspectedType.IsAssignableFrom(drawnType))
//                 {
//                     return (bool)EditorForChildClassesField.GetValue(attribute);
//                 }
//             }
//
//             return false;
//         }
//
//         /// <summary>
//         /// 通过从编辑器的 CustomEditor 特性（如果已声明）中提取，获取编辑器绘制的类型。
//         /// </summary>
//         public static Type GetEditorDrawnType(Type editorType, out bool editorForChildClasses)
//         {
//             if (editorType == null)
//             {
//                 throw new ArgumentNullException(nameof(editorType));
//             }
//
//             editorForChildClasses = false;
//
//             if (editorType.IsAbstract || InspectedTypeField == null ||
//                 EditorForChildClassesField == null)
//             {
//                 return null;
//             }
//
//             // 临时禁用日志以避免在获取属性时产生不必要的警告
//             bool previous = Debug.unityLogger.logEnabled;
//             Debug.unityLogger.logEnabled = false;
//             var customEditorAttribute = editorType.GetCustomAttribute<CustomEditor>();
//             Debug.unityLogger.logEnabled = previous;
//
//             if (customEditorAttribute != null)
//             {
//                 editorForChildClasses = (bool)EditorForChildClassesField.GetValue(customEditorAttribute);
//                 return (Type)InspectedTypeField.GetValue(customEditorAttribute);
//             }
//
//             return null;
//         }
//     }
// } 
