using System;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    public class TerrainObject : SerializedMonoBehaviour
    {
        [OdinSerialize] private Guid _targetTerrainTileDefinitionGuid;

        public Guid TargetTerrainTileDefinitionGuid
        {
            get => _targetTerrainTileDefinitionGuid;
            set => _targetTerrainTileDefinitionGuid = value;
        }


        public void ClearTiles()
        {
            var childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(0);
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}