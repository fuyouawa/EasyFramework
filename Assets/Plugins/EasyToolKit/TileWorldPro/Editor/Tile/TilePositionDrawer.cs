using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.TileWorldPro;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value + 100)]
    public class TilePositionDrawer : EasyValueDrawer<TilePosition>
    {
        private InspectorProperty _positionProperty;

        protected override void Initialize()
        {
            base.Initialize();
            _positionProperty = Property.Children["_position"];
        }

        protected override void DrawProperty(GUIContent label)
        {
            _positionProperty.Draw(label);
        }
    }
}