using EasyToolKit.Core.Editor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public static class InspectorPropertyExtensions
    {
        private static readonly GUIContent TempContent = new GUIContent();

        public static void Draw(this InspectorProperty property, string label, string tooltip = null)
        {
            property.Draw(TempContent.SetText(label).SetTooltip(tooltip));
        }
    }
}
