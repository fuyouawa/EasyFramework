using System;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using EasyFramework.Inspector;
using Sirenix.Serialization;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace EasyFramework.Tools.Editor
{
    public partial class UiTextManagerWindow : OdinMenuEditorWindow
    {
        public class WindowTemp
        {
            public Rect WindowPosition;
            public float MenuWidth;
        }

        [Flags]
        public enum ViewModes
        {
            Correct = 1,
            Incorrect = 1 << 2,
            All = Correct | Incorrect,
        }

        public enum ManagerPositions
        {
            InScene,
            InProject
        }

        private static UiTextManagerWindow s_instance;

        public static UiTextManagerWindow Instance
        {
            get
            {
                if (s_instance == null)
                {
                    Debug.Assert(HasOpenInstances<UiTextManagerWindow>());
                    s_instance = GetWindow<UiTextManagerWindow>("Ui Text Manager");
                }

                Debug.Assert(s_instance != null);
                return s_instance;
            }
        }

        [MenuItem("Tools/EasyGameFramework/Tools/Ui Text Manager")]
        public static void ShowWindow()
        {
            var isNew = HasOpenInstances<UiTextManagerWindow>();
            var window = GetWindow<UiTextManagerWindow>("Ui Text Manager");
            if (!isNew)
            {
                if (!File.Exists(EditorAssetsPath.UiTextManagerWindowTempPath))
                {
                    window.CenterWindowWithSizeRadio(new Vector2(0.4f, 0.45f));
                }
            }

            s_instance = window;
        }

        private static UiTextManagerSettings Settings => UiTextManagerSettings.Instance;
        private PrefabItemBase _prevSelectionItem;

        public void Rebuild()
        {
            ForceMenuTreeRebuild();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false, OdinMenuStyle.TreeViewStyle)
            {
                { "设置", Settings, SdfIconType.GearFill },
                { UiTextManagerHelper.Group, null }
            };

            if (Settings.ManagerPosition == ManagerPositions.InProject)
            {
                UiTextManagerHelper.LoadPrefabsInProject(tree);
            }
            else
            {
                UiTextManagerHelper.LoadPrefabsInScene(tree);
            }

            tree.Selection.SelectionChanged += type =>
            {
                if (type == SelectionChangedType.ItemAdded)
                {
                    if (tree.Selection.SelectedValue is PrefabItemBase item)
                    {
                        _prevSelectionItem?.ClearCache();
                        _prevSelectionItem = item;
                        item.Analyse();
                        if (Settings.AutoOpenSelectionPrefab)
                        {
                            item.OpenPrefab();
                        }
                    }
                }
            };
            return tree;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EditorApplication.update += EditorUpdate;

            LoadEditor();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EditorApplication.update -= EditorUpdate;
            SaveEditor();
        }

        private double _prevAutoSaveTime;

        private void EditorUpdate()
        {
            if (EditorApplication.timeSinceStartup - _prevAutoSaveTime >= 1f)
            {
                SaveEditor();
                _prevAutoSaveTime = EditorApplication.timeSinceStartup;
            }
        }

        public void SaveEditor()
        {
            var temp = new WindowTemp
            {
                WindowPosition = position,
                MenuWidth = MenuWidth
            };

            var json = SerializationUtility.SerializeValue(temp, DataFormat.JSON);
            File.WriteAllBytes(EditorAssetsPath.UiTextManagerWindowTempPath, json);
        }

        public void LoadEditor()
        {
            if (!File.Exists(EditorAssetsPath.UiTextManagerWindowTempPath))
                return;
            var json = File.ReadAllBytes(EditorAssetsPath.UiTextManagerWindowTempPath);
            var temp = SerializationUtility.DeserializeValue<WindowTemp>(json, DataFormat.JSON);
            if (temp != null)
            {
                position = temp.WindowPosition;
                MenuWidth = temp.MenuWidth;
            }
        }
    }
}
