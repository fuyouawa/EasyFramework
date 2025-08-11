using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public class BeforeTileInstantiateParameters
    {
        public TileWorldBuilder Builder { get; }
        public ChunkObject ChunkObject { get; }
        public Guid TerrainGuid { get; set; }
        public ChunkTilePosition ChunkTilePosition { get; set; }
        public TerrainTileRuleType RuleType { get; set; }

        public BeforeTileInstantiateParameters(TileWorldBuilder builder, ChunkObject chunkObject, Guid terrainGuid, ChunkTilePosition chunkTilePosition, TerrainTileRuleType ruleType)
        {
            Builder = builder;
            ChunkObject = chunkObject;
            TerrainGuid = terrainGuid;
            ChunkTilePosition = chunkTilePosition;
            RuleType = ruleType;
        }
    }

    public class AfterTileInstantiateParameters
    {
        public TileWorldBuilder Builder { get; }
        public ChunkObject ChunkObject { get; }
        public GameObject TileInstance { get; set; }

        public AfterTileInstantiateParameters(TileWorldBuilder builder, ChunkObject chunkObject, GameObject tileInstance)
        {
            Builder = builder;
            ChunkObject = chunkObject;
            TileInstance = tileInstance;
        }
    }

    public interface ITileBuildProcessor
    {
        void OnBeforeInstantiateTile(BeforeTileInstantiateParameters parameters);
        void OnAfterInstantiateTile(AfterTileInstantiateParameters parameters);
    }
}