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
        private TileWorldBuilder _target;

        protected override void OnEnable()
        {
            base.OnEnable();
            _target = (TileWorldBuilder)target;
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
                ((TileWorldBuilder)target).BuildAll();
            }
            Tree.EndDraw();
        }

        void IEasyEventListener<SetTilesEvent>.OnEvent(object sender, SetTilesEvent eventArg)
        {
            if (_target.Settings.RealTimeIncrementalBuild)
            {
                _target.BuildTiles(eventArg.TerrainGuid, eventArg.TilePositions);
            }
        }

        void IEasyEventListener<RemoveTilesEvent>.OnEvent(object sender, RemoveTilesEvent eventArg)
        {
            if (_target.Settings.RealTimeIncrementalBuild)
            {
                _target.DestroyTiles(eventArg.TerrainGuid, eventArg.TilePositions);
            }
        }
    }
}