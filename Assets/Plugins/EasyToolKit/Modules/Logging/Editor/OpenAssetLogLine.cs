// using System.Reflection;
// using System.Text.RegularExpressions;
// using System;
// using UnityEditor.Callbacks;
// using UnityEditor;
//
// namespace EasyFramework.ToolKit.Editor
// {
//     // 实现在控制台中双击日志时重定位打开脚本
//     // 参考: https://zhuanlan.zhihu.com/p/92291084
//     public static class OpenAssetLogLine
//     {
//         private static bool s_hasForceMono = false;
//
//         [OnOpenAsset(-1)]
//         static bool OnOpenAsset(int instance, int line)
//         {
//             if (s_hasForceMono) return false;
//
//             // 自定义函数，用来获取log中的stacktrace，定义在后面。
//             string stackTrace = GetStackTrace();
//             var signature = "Log/Core/Log.cs";
//
//             if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Contains(signature))
//             {
//                 var matches = Regex.Matches(stackTrace, @"\(at (.+)\)", RegexOptions.IgnoreCase);
//
//                 int i = 0;
//                 for (; i < matches.Count; i++)
//                 {
//                     var m = matches[i];
//                     var p = m.Groups[1].Value;
//                     if (p.Contains(signature))
//                     {
//                         break;
//                     }
//                 }
//
//                 if (i >= matches.Count - 1)
//                     return false;
//
//                 var match = matches[i + 1];
//                 var pathLine = match.Groups[1].Value;
//                 int splitIndex = pathLine.LastIndexOf(":", StringComparison.Ordinal);
//                 var path = pathLine[0..splitIndex];
//                 line = Convert.ToInt32(pathLine[(splitIndex + 1)..]);
//
//                 s_hasForceMono = true;
//                 AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path), line);
//                 s_hasForceMono = false;
//                 return true;
//             }
//
//             return false;
//         }
//
//         private static Type s_consoleWindowType;
//
//         private static Type ConsoleWindowType =>
//             s_consoleWindowType ??= typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
//
//         private static FieldInfo s_consoleWindowInstanceField;
//
//         private static FieldInfo ConsoleWindowInstanceField =>
//             s_consoleWindowInstanceField ??=
//                 ConsoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
//
//         private static FieldInfo s_consoleWindowActiveTextField;
//
//         private static FieldInfo ConsoleWindowActiveTextField =>
//             s_consoleWindowActiveTextField ??=
//                 ConsoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
//
//         static string GetStackTrace()
//         {
//             var instance = ConsoleWindowInstanceField.GetValue(null) as EditorWindow;
//             if (instance != null)
//             {
//                 if (EditorWindow.focusedWindow == instance)
//                 {
//                     string activeText = ConsoleWindowActiveTextField.GetValue(instance).ToString();
//                     return activeText;
//                 }
//             }
//
//             return null;
//         }
//     }
// }
