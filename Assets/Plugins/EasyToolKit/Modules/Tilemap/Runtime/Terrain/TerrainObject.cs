using System;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    public class TerrainObject : SerializedMonoBehaviour
    {
        [OdinSerialize, ShowInInspector, ReadOnly] private Guid _targetTerrainTileDefinitionGuid;

        public Guid TargetTerrainTileDefinitionGuid
        {
            get => _targetTerrainTileDefinitionGuid;
            set => _targetTerrainTileDefinitionGuid = value;
        }


        public void ClearTiles()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}