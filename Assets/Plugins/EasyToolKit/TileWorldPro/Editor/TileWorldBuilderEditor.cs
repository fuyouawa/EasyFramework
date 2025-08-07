using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.TileWorldPro;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    [CustomEditor(typeof(TileWorldBuilder))]
    public class TileWorldBuilderEditor : EasyEditor, IEasyEventListener<SetTilesEvent>, IEasyEventListener<RemoveTilesEvent>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            this.RegisterListener<SetTilesEvent>();
            this.RegisterListener<RemoveTilesEvent>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.UnregisterListener<SetTilesEvent>();
            this.UnregisterListener<RemoveTilesEvent>();
        }

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

        void IEasyEventListener<SetTilesEvent>.OnEvent(object sender, SetTilesEvent eventArg)
        {
            Debug.Log($"SetTilesEvent: {eventArg.TerrainGuid}, {eventArg.TilePositions.Length}");
        }

        void IEasyEventListener<RemoveTilesEvent>.OnEvent(object sender, RemoveTilesEvent eventArg)
        {
            Debug.Log($"RemoveTilesEvent: {eventArg.TerrainGuid}, {eventArg.TilePositions.Length}");
        }
    }
}