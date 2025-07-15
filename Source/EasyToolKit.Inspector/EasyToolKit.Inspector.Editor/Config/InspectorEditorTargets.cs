// using System;
//
// namespace EasyToolKit.Inspector.Editor
// {
//     [Flags]
//     public enum InspectorEditorTargets
//     {
//         /// <summary>
//         /// 排除所有类型。
//         /// </summary>
//         None = 0,
//
//         /// <summary>
//         /// 用户类型包括所有不位于编辑器或插件文件夹中的自定义用户脚本。
//         /// </summary>
//         UserTypes = 1 << 0,
//
//         /// <summary>
//         /// 插件类型包括位于插件文件夹中且不在编辑器文件夹中的所有类型。
//         /// </summary>
//         PluginTypes = 1 << 1,
//
//         /// <summary>
//         /// Unity类型包括所有依赖于UnityEngine和来自UnityEngine的类型，除了编辑器、插件和用户类型。
//         /// </summary>
//         UnityTypes = 1 << 2,
//
//         /// <summary>
//         /// 其他类型包括所有不依赖于UnityEngine或UnityEditor的其他类型。
//         /// </summary>
//         OtherTypes = 1 << 3
//     }
// }
