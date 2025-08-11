using System;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.TileWorldPro;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    [CustomEditor(typeof(TileWorldAsset))]
    public class TileWorldAssetEditor : EasyEditor
    {
        public static bool IsInDesigner = false;

        private static ITileWorldDataStore _temporaryDataStore;
        private readonly static GUIContent TempContent = new GUIContent();

        private TileWorldAsset _target;

        private InspectorProperty _tileSizeProperty;
        private InspectorProperty _chunkSizeProperty;
        private InspectorProperty _terrainDefinitionSetProperty;
        private InspectorProperty _dataStoreProperty;
        private bool _isInitialized = false;

        private LocalPersistentContext<bool> _dataStoreExpanded;

        protected override void OnEnable()
        {
            base.OnEnable();
            _target = (TileWorldAsset)target;

            _dataStoreExpanded = Tree.LogicRootProperty.GetPersistentContext(nameof(_dataStoreExpanded), true);
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw();

            if (!_isInitialized)
            {
                _tileSizeProperty = Tree.LogicRootProperty.Children["_tileSize"];
                _chunkSizeProperty = Tree.LogicRootProperty.Children["_chunkSize"];
                _terrainDefinitionSetProperty = Tree.LogicRootProperty.Children["_terrainDefinitionSet"];
                _dataStoreProperty = Tree.LogicRootProperty.Children["_dataStore"];
                _isInitialized = true;
            }

            _tileSizeProperty.Draw();
            _chunkSizeProperty.Draw();

            if (IsInDesigner)
            {
                _terrainDefinitionSetProperty.Draw();
            }

            var dataStoreNames = TileWorldDataStoreUtility.GetDataStoreNamesCache();
            int selectedDataStoreIndex = -1;
            if (_target.DataStore != null)
            {
                var dataStoreName = TileWorldDataStoreUtility.GetDataStoreNameByType(_target.DataStore.GetType());
                selectedDataStoreIndex = Array.IndexOf(dataStoreNames, dataStoreName);
            }
            EditorGUI.BeginChangeCheck();
            selectedDataStoreIndex = EditorGUILayout.Popup("数据存储方案", selectedDataStoreIndex, dataStoreNames);
            if (EditorGUI.EndChangeCheck())
            {
                if (selectedDataStoreIndex != -1)
                {
                    var dataStoreType = TileWorldDataStoreUtility.GetDataStoreTypeByName(dataStoreNames[selectedDataStoreIndex]);
                    _temporaryDataStore = _target.DataStore;
                    _target.DataStore = Activator.CreateInstance(dataStoreType) as ITileWorldDataStore;
                }
                else
                {
                    _target.DataStore = null;
                }
                    _dataStoreProperty.Refresh();
            }

            if (_target.DataStore != null)
            {
                EasyEditorGUI.BeginBox();
                EasyEditorGUI.BeginBoxHeader();
                _dataStoreExpanded.Value = EasyEditorGUI.Foldout(_dataStoreExpanded.Value, TempContent.SetText($"数据存储 - {dataStoreNames[selectedDataStoreIndex]}"));
                EasyEditorGUI.EndBoxHeader();

                if (_dataStoreExpanded.Value)
                {
                    _dataStoreProperty.Draw();
                }
                EasyEditorGUI.EndBox();

                if (_temporaryDataStore != null)
                {
                    if (GUILayout.Button("转移数据"))
                    {
                        _target.DataStore.TransferData(_temporaryDataStore);
                        _temporaryDataStore.Dispose();
                        _temporaryDataStore = null;
                    }
                }
            }
            Tree.EndDraw();
        }
    }
}