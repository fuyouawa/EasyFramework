using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.TileWorldPro;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    [CustomEditor(typeof(TileWorldBuilder))]
    public class TileWorldBuilderEditor : EasyEditor
    {
        protected override void DrawTree()
        {
            Tree.BeginDraw();
            Tree.DrawProperties();

            if (GUILayout.Button("重新构建"))
            {
                ((TileWorldBuilder)target).RebuildAll();
            }
            Tree.EndDraw();
        }
    }
}