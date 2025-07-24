using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class EasyGroupAttributeDrawer<TAttribute> : EasyAttributeDrawer<TAttribute>
        where TAttribute : BeginGroupAttribute
    {
        protected override void DrawProperty(GUIContent label)
        {
            bool foldout = true;
            BeginDrawProperty(label, ref foldout);
            if (foldout)
            {
                CallNextDrawer(label);
                foreach (var groupProperty in Property.GetGroupProperties(typeof(TAttribute)))
                {
                    groupProperty.Draw();
                    groupProperty.SkipDrawCount++;
                }
            }
            else
            {
                foreach (var groupProperty in Property.GetGroupProperties(typeof(TAttribute)))
                {
                    groupProperty.SkipDrawCount++;
                }
            }

            EndDrawProperty();
        }

        protected virtual void BeginDrawProperty(GUIContent label, ref bool foldout)
        {
        }

        protected virtual void EndDrawProperty()
        {
        }
    }
}
