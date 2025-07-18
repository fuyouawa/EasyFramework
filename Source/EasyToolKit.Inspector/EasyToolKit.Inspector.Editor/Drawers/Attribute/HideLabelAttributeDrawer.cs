using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class HideLabelAttributeDrawer : EasyAttributeDrawer<HideLabelAttribute>
    {
        protected override void OnDrawProperty(GUIContent label)
        {
            CallNextDrawer(null);
        }
    }
}
