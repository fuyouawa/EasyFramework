using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class HideLabelAttributeDrawer : EasyAttributeDrawer<HideLabelAttribute>
    {
        protected override void DrawProperty(GUIContent label)
        {
            CallNextDrawer(null);
        }
    }
}
