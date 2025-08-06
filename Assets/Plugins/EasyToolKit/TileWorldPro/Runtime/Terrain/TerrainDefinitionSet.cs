using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public class TerrainDefinitionSet : IEnumerable<TerrainDefinition>
    {
        [MetroListDrawerSettings(OnRemovedElementCallback = nameof(TriggerRemovedDefinition))]
        [LabelText("地形瓦片定义表")]
        [SerializeField]
        private List<TerrainDefinition> _definitions = new List<TerrainDefinition>();

        public List<TerrainDefinition> Definitions => _definitions;

#if UNITY_EDITOR
        public event Action<TerrainDefinition> OnRemovedDefinition;
#endif

        public IEnumerator<TerrainDefinition> GetEnumerator()
        {
            return _definitions.GetEnumerator();
        }

        public TerrainDefinition TryGetByGuid(Guid guid)
        {
            return Definitions.FirstOrDefault(terrainTile => terrainTile.Guid == guid);
        }

        public bool Contains(Guid guid)
        {
            if (_definitions.Count == 0)
            {
                return false;
            }

            return _definitions.Any(definition => definition.Guid == guid);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#if UNITY_EDITOR
        private void TriggerRemovedDefinition(object weakDefinition)
        {
            var definition = weakDefinition as TerrainDefinition;
            OnRemovedDefinition?.Invoke(definition);
        }
#endif
    }
}
