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
                var groupProperties = Property.GetGroupProperties(typeof(TAttribute));
                foreach (var groupProperty in groupProperties)
                {
                    groupProperty.Draw();
                    groupProperty.SkipDrawCount++;
                }
            }
            else
            {
                var groupProperties = Property.GetGroupProperties(typeof(TAttribute));
                foreach (var groupProperty in groupProperties)
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
