using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [CustomEditor(typeof(PersistentContextCacheAsset))]
    public class PersistentContextCacheAssetEditor : EasyEditor
    {
        private PersistentContextCacheAsset _target;

        protected override void OnEnable()
        {
            base.OnEnable();
            _target = (PersistentContextCacheAsset)target;
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw();

            EditorGUILayout.LabelField($"Context Cache Count", $"{_target.CacheCount}");
            EditorGUILayout.LabelField($"Context Cache Data Size", $"{(_target.TotalCacheDataSize / 1024f):F2} KB");

            Tree.DrawProperties();

            if (GUILayout.Button("Clear Cache"))
            {
                _target.ClearCache();
            }

            Tree.EndDraw();
        }
    }
}