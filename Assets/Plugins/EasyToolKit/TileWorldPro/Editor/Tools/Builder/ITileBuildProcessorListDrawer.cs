using System.Collections.Generic;
using EasyToolKit.Inspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value + 9.1)]
    public class ITileBuildProcessorListDrawer : CollectionDrawer<IList<ITileBuildProcessor>>
    {
        protected override void DrawElementProperty(InspectorProperty property, int index)
        {
            property.Draw(null);
        }
    }
}