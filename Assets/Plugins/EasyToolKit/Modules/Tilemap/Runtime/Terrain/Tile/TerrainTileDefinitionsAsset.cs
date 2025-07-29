using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [CreateAssetMenu(menuName = "EasyToolKit/Tilemap/Create TerrainTileDefinitionsAsset", fileName = "TerrainTileDefinitionsAsset")]
    [EasyInspector]
    public class TerrainTileDefinitionsAsset : ScriptableObject, IEnumerable<TerrainTileDefinition>
    {
        [MetroListDrawerSettings(OnRemovedElementCallback = nameof(TriggerRemovedDefinition))]
        [LabelText("地形瓦片定义表")]
        [SerializeField]
        private List<TerrainTileDefinition> _definitions = new List<TerrainTileDefinition>();

        public List<TerrainTileDefinition> Definitions => _definitions;
        public event Action<TerrainTileDefinition> OnRemovedDefinition;

        public IEnumerator<TerrainTileDefinition> GetEnumerator()
        {
            return _definitions.GetEnumerator();
        }

        public TerrainTileDefinition TryGetByGuid(Guid guid)
        {
            return Definitions.FirstOrDefault(terrainTile => terrainTile.Guid == guid);
        }

        public bool Contains(Guid guid)
        {
            return _definitions.Any(definition => definition.Guid == guid);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void TriggerRemovedDefinition(object weakDefinition)
        {
            var definition = weakDefinition as TerrainTileDefinition;
            OnRemovedDefinition?.Invoke(definition);
        }
    }
}