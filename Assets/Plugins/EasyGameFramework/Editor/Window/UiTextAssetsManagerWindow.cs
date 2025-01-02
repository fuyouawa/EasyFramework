using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using TMPro;
using EasyFramework;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    public class UiTextAssetsManagerWindowTemp
    {
        public Rect WindowPosition;
        public float MenuWidth;
    }

    public class UiTextAssetsManagerWindow : OdinMenuEditorWindow
    {
        private static UiTextAssetsManagerWindow _instance;

        public static UiTextAssetsManagerWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.Assert(HasOpenInstances<UiTextAssetsManagerWindow>());
                    _instance = GetWindow<UiTextAssetsManagerWindow>("UiTextAssetsManager");
                }
                Debug.Assert(_instance != null);
                return _instance;
            }
        }
        
        [MenuItem("Tools/EasyGameFramework/Open UiTextAssetsManager")]
        public static void ShowWindow()
        {
            var isNew = HasOpenInstances<UiTextAssetsManagerWindow>();
            var window = GetWindow<UiTextAssetsManagerWindow>("UiTextAssetsManager");
            if (!isNew)
            {
                if (!File.Exists(EditorAssetsPath.UiTextAssetsManagerWindowTempPath))
                {
                    window.CenterWindowWithSizeRadio(new Vector2(0.4f, 0.45f));
                }
            }
            _instance = window;
        }

        private UiTextAssetsEditorSettings _settings = new();
        private UiTextPrefabAsset _prevSelectionItem;

        public void Rebuild()
        {
            ForceMenuTreeRebuild();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var assetsPath = $"Assets/{_settings.AssetsManagerPath}/";
            var prefix = "资产视图";
            var tree = new OdinMenuTree()
            {
                { "设置", _settings, SdfIconType.GearFill },
                { prefix, null }
            };
            tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;

            if (_settings.AssetsManagerPath.IsNotNullOrEmpty())
            {
                var allPrefabs = AssetDatabase.GetAllAssetPaths()
                    .Where(p => p.StartsWith(assetsPath) &&
                                p.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase));
                foreach (var path in allPrefabs)
                {
                    var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (obj != null && obj.GetComponentInChildren<TextMeshProUGUI>() != null)
                    {
                        var item = new UiTextPrefabAsset(path);
                        
                        item.Analyse();
                        if (AssetItemFilter(item))
                        {
                            Texture2D icon;
                            if (item.HasWarn())
                            {
                                icon = EditorIcons.UnityWarningIcon;
                            }
                            else
                            {
                                icon = InternalEditorUtility.GetIconForFile(path);
                            }
                            
                            var p = path.Substring(assetsPath.Length, path.Length - assetsPath.Length - ".prefab".Length);
                            tree.Add($"{prefix}/{p}", item, icon);
                        }
                        item.ClearCache();
                    }
                }

                var items = tree.MenuItems.First(i => i.Name == prefix).GetChildMenuItemsRecursive(false);
                foreach (var item in items)
                {
                    if (item.Value == null)
                    {
                        item.Icon = EditorIcons.UnityFolderIcon;
                    }
                }
            }

            tree.Selection.SupportsMultiSelect = false;
            tree.Selection.SelectionChanged += type =>
            {
                if (type == SelectionChangedType.ItemAdded)
                {
                    if (tree.Selection.SelectedValue is UiTextPrefabAsset item)
                    {
                        _prevSelectionItem?.ClearCache();
                        _prevSelectionItem = item;
                        item.Analyse();
                        if (_settings.AutoOpenSelectionPrefab)
                        {
                            item.OpenPrefab();
                        }
                    }
                }
            };
            return tree;
        }


        private bool AssetItemFilter(UiTextPrefabAsset item)
        {
            if (_settings == null)
                return true;
            if (_settings.ViewModes == 0)
                return false;
            if (_settings.ViewModes == PrefabViewModes.All)
                return true;
            
            var warn = item.HasWarn();
            
            if (warn && _settings.ViewModes.HasFlag(PrefabViewModes.Warn))
            {
                return true;
            }

            if (!warn && _settings.ViewModes.HasFlag(PrefabViewModes.Correct))
            {
                return true;
            }

            return false;
        }
        

        protected override void OnEnable()
        {
            base.OnEnable();
            EditorApplication.update += EditorUpdate;
            
            Load();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EditorApplication.update -= EditorUpdate;
            Save();
        }

        private double _prevAutoSaveTime;

        private void EditorUpdate()
        {
            if (EditorApplication.timeSinceStartup - _prevAutoSaveTime >= _settings.AutoSaveInterval)
            {
                Save();
                _prevAutoSaveTime = EditorApplication.timeSinceStartup;
            }
        }
        
        public void Save()
        {
            var json = EasyJsonConvert.SerializeObject(_settings, Formatting.Indented);
            File.WriteAllText(EditorAssetsPath.UiTextAssetsEditorSettingsPath, json);

            var temp = new UiTextAssetsManagerWindowTemp
            {
                WindowPosition = position,
                MenuWidth = MenuWidth
            };
            json = EasyJsonConvert.SerializeObject(temp, Formatting.Indented);
            File.WriteAllText(EditorAssetsPath.UiTextAssetsManagerWindowTempPath, json);
        }

        public void Load()
        {
            if (!File.Exists(EditorAssetsPath.UiTextAssetsEditorSettingsPath))
                return;
            var json = File.ReadAllText(EditorAssetsPath.UiTextAssetsEditorSettingsPath);
            var val = EasyJsonConvert.DeserializeObject<UiTextAssetsEditorSettings>(json);
            if (val != null)
            {
                _settings = val;
            }
            
            if (!File.Exists(EditorAssetsPath.UiTextAssetsManagerWindowTempPath))
                return;
            json = File.ReadAllText(EditorAssetsPath.UiTextAssetsManagerWindowTempPath);
            var temp = EasyJsonConvert.DeserializeObject<UiTextAssetsManagerWindowTemp>(json);
            if (temp != null)
            {
                position = temp.WindowPosition;
                MenuWidth = temp.MenuWidth;
            }
        }
    }
}
