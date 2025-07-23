using System;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    internal static class CustomEditorLoader
    {
        [DidReloadScripts]
        static CustomEditorLoader()
        {
            InspectorConfig.Instance.UpdateEditors();
        }
    }
}
