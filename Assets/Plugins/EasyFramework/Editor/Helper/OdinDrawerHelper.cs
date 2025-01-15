using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    public static class OdinDrawerHelper
    {
        public static bool CallNextDrawer(OdinDrawer drawer, OdinDrawer nextDrawer, GUIContent label)
        {
            if (nextDrawer != null)
            {
                nextDrawer.DrawProperty(label);
                return true;
            }
            if (drawer.Property.ValueEntry != null)
            {
                Rect rect = EditorGUILayout.GetControlRect();
                if (label == null)
                {
                    GUI.Label(rect, drawer.Property.NiceName);
                }
                else
                {
                    GUI.Label(rect, label);
                }
            }
            else
            {
                GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
                if (label != null)
                {
                    EditorGUILayout.PrefixLabel(label);
                }
                SirenixEditorGUI.WarningMessageBox("There is no drawer defined for property " + drawer.Property.NiceName + " of type " + drawer.Property.Info.PropertyType.ToString() + ".");
                GUILayout.EndHorizontal();
            }
            return false;
        }

        public static bool CallNextDrawer(OdinDrawer drawer, GUIContent label)
        {
            OdinDrawer nextDrawer = null;
            BakedDrawerChain chain = drawer.Property.GetActiveDrawerChain();
            if (chain.MoveNext())
            {
                nextDrawer = chain.Current;
            }

            return CallNextDrawer(drawer, nextDrawer, label);
        }
    }
}
