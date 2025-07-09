using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [InitializeOnLoad]
    [CanEditMultipleObjects]
    public class EasyEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
