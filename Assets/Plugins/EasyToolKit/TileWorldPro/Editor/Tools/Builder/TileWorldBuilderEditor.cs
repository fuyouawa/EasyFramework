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

        void IEasyEventListener<SetTilesEvent>.OnEvent(object sender, SetTilesEvent eventArg)
        {
            if (_target.Settings.RealTimeIncrementalBuild)
            {
                _target.RebuildTiles(eventArg.TerrainGuid, eventArg.TilePositions);
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