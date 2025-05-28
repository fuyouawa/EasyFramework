// using System;
// using System.Linq;
// using EasyFramework.Editor;
// using Sirenix.OdinInspector.Editor;
// using Sirenix.Utilities.Editor;
// using TMPro;
// using UnityEditor;
// using UnityEngine.SceneManagement;
// using UnityEngine;
//
// namespace EasyFramework.ToolKit.Editor
// {
//     public static class UiTextManagerHelper
//     {
//         private static UiTextManagerSettings Settings => UiTextManagerSettings.Instance;
//         
//         public const string Group = "资产视图";
//
//         public static void LoadPrefabsInScene(OdinMenuTree tree)
//         {
//             for (int i = 0; i < SceneManager.sceneCount; i++)
//             {
//                 var scene = SceneManager.GetSceneAt(i);
//                 if (scene.IsValid() && scene.isLoaded)
//                 {
//                     tree.Add($"{Group}/{scene.name}", null, EditorIcons.UnityLogo);
//
//                     foreach (var o in scene.GetRootGameObjects())
//                     {
//                         if (o.GetComponentsInChildren<TextMeshProUGUI>(true).IsNotNullOrEmpty())
//                         {
//                             var item = new UiTextManagerWindow.PrefabSceneItem(o, $"{scene.name}/{o.name}");
//
//                             item.Analyse();
//                             if (AssetItemFilter(item))
//                             {
//                                 var icon = item.HasIncorrect()
//                                     ? EditorIcons.UnityWarningIcon
//                                     : EasyEditorIcons.UnityPrefabIcon;
//
//                                 tree.Add($"{Group}/{scene.name}/{o.name}", item, icon);
//                             }
//
//                             item.ClearCache();
//                         }
//                     }
//                 }
//             }
//         }
//
//
//         public static void LoadPrefabsInProject(OdinMenuTree tree)
//         {
//             var assetsPath = $"Assets/{Settings.AssetsManagerPath}/";
//
//             if (Settings.AssetsManagerPath.IsNotNullOrEmpty())
//             {
//                 var allPrefabs = AssetDatabase.GetAllAssetPaths()
//                     .Where(p => p.StartsWith(assetsPath) &&
//                                 p.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase));
//                 foreach (var path in allPrefabs)
//                 {
//                     var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
//                     if (obj != null && obj.GetComponentInChildren<TextMeshProUGUI>() != null)
//                     {
//                         var item = new UiTextManagerWindow.PrefabAssetItem(path);
//
//                         item.Analyse();
//                         if (AssetItemFilter(item))
//                         {
//                             var icon = item.HasIncorrect()
//                                 ? EditorIcons.UnityWarningIcon
//                                 : EasyEditorIcons.UnityPrefabIcon;
//                             var p = path.Substring(assetsPath.Length,
//                                 path.Length - assetsPath.Length - ".prefab".Length);
//                             var menuItems = tree.Add($"{Group}/{p}", item, icon).ToArray();
//                             foreach (var menuItem in menuItems)
//                             {
//                                 menuItem.Icon = menuItem.Name != obj.name
//                                     ? EditorIcons.UnityFolderIcon
//                                     : icon;
//                             }
//                         }
//
//                         item.ClearCache();
//                     }
//                 }
//
//                 var items = tree.MenuItems.First(i => i.Name == Group).GetChildMenuItemsRecursive(false);
//                 foreach (var item in items)
//                 {
//                     if (item.Value == null)
//                     {
//                         item.Icon = EditorIcons.UnityFolderIcon;
//                     }
//                 }
//             }
//         }
//
//
//         public static bool AssetItemFilter(UiTextManagerWindow.PrefabItemBase item)
//         {
//             if (Settings == null)
//                 return true;
//             if (Settings.ViewModes == 0)
//                 return false;
//             if (Settings.ViewModes == UiTextManagerWindow.ViewModes.All)
//                 return true;
//
//             var warn = item.HasIncorrect();
//
//             if (warn && Settings.ViewModes.HasFlag(UiTextManagerWindow.ViewModes.Incorrect))
//             {
//                 return true;
//             }
//
//             if (!warn && Settings.ViewModes.HasFlag(UiTextManagerWindow.ViewModes.Correct))
//             {
//                 return true;
//             }
//
//             return false;
//         }
//
//         public static bool GameObjectHasIncorrect(GameObject obj)
//         {
//             var mgrs = obj.GetComponents<UiTextManager>();
//             if (mgrs.IsNullOrEmpty() || mgrs.Length > 1)
//                 return true;
//             var mgr = mgrs[0];
//             if (mgr.GetFontAssetPreset() == null || mgr.GetTextPropertiesPreset() == null)
//                 return true;
//
//             return false;
//         }
//     }
// }
