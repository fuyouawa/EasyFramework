using System;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.TileWorldPro;
using UnityEditor;

namespace EasyToolKit.TileWorldPro.Editor
{
    [CustomEditor(typeof(TileWorldAsset))]
    public class TileWorldAssetEditor : EasyEditor
    {
        private TileWorldAsset _target;

        protected override void OnEnable()
        {
            base.OnEnable();
            _target = (TileWorldAsset)target;
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw();
            Tree.DrawProperties();
            if (_target.DataStore == null)
            {
                var dataStoreNames = TileWorldDataStoreUtility.GetDataStoreNamesCache();
                var dataStoreName = EditorGUILayout.Popup("数据存储", -1, dataStoreNames);
                if (dataStoreName != -1)
                {
                    var dataStoreType = TileWorldDataStoreUtility.GetDataStoreType(dataStoreNames[dataStoreName]);
                    _target.DataStore = Activator.CreateInstance(dataStoreType) as ITileWorldDataStore;
                }
            }
            Tree.EndDraw();
        }
    }
}