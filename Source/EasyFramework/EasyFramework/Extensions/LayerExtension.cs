using UnityEngine;

namespace EasyFramework
{
    public static class LayerExtension
    {
        public static bool Contains(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }
        public static bool Contains(this LayerMask mask, string layer)
        {
            return mask.Contains(LayerMask.NameToLayer(layer));
        }

        public static LayerMask FromLayers(params int[] layers)
        {
            int mask = 0;
            foreach (var layer in layers)
            {
                mask |= (1 << layer);
            }
            return mask;
        }
        public static LayerMask FromLayers(params string[] layers)
        {
            int mask = 0;
            foreach (var layer in layers)
            {
                mask |= (1 << LayerMask.NameToLayer(layer));
            }
            return mask;
        }
    }
}
