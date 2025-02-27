using System;
using System.Linq;
using EasyFramework.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [CustomEditor(typeof(Transform), true)]
    [CanEditMultipleObjects]
    public class TransformEditor : ExtendEditor<Transform>
    {
        protected override string EditorTypeName => "UnityEditor.TransformInspector, UnityEditor";

        class Contents
        {
            public readonly GUIContent PositionContent = EditorGUIUtility.TrTextContent("Position");
            public readonly GUIContent RotationContent = EditorGUIUtility.TrTextContent("Rotation");
            public readonly GUIContent ScaleContent = EditorGUIUtility.TrTextContent("Scale");
        }

        private readonly ComponentPreviewPanel _componentPreviewPanel = new ComponentPreviewPanel();
        protected override void OnEnable()
        {
            base.OnEnable();
            _componentPreviewPanel.OnEnable(Target.gameObject, targets.Select(t =>
            {
                if (!(t is Transform t2))
                    throw new Exception("T is different from the type in the CustomEditor attribute");
                return t2.gameObject;
            }));
        }

        private static Contents s_contents;
        public override void OnInspectorGUI()
        {
            s_contents ??= new Contents();

            _componentPreviewPanel.OnHeaderGUI();
            serializedObject.ApplyModifiedProperties();

            SirenixEditorGUI.Title("局部空间", string.Empty, TextAlignment.Left, true);
            // EditorGUILayout.LabelField("Local Space", EditorStyles.boldLabel);

            base.OnInspectorGUI();
            
            SirenixEditorGUI.Title("世界空间", string.Empty, TextAlignment.Left, true);
            // EditorGUILayout.LabelField("World", EditorStyles.boldLabel);

            var v = Target.position;
            var w = EditorGUIUtility.singleLineHeight;
            using (new EditorGUILayout.HorizontalScope())
            {
                var v2 = EditorGUILayout.Vector3Field(s_contents.PositionContent, v);
                if (GUILayout.Button("Z", GUILayout.Width(w)))
                    v2 = Vector3.zero;
                if (v != v2)
                    Target.position = v2;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                v = Target.rotation.eulerAngles;
                var v2 = EditorGUILayout.Vector3Field(s_contents.RotationContent, v);
                if (GUILayout.Button("Z", GUILayout.Width(w)))
                    v2 = Vector3.zero;
                if (v != v2)
                    Target.rotation = Quaternion.Euler(v2);
            }


            v = Target.lossyScale;
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.Vector3Field(s_contents.ScaleContent, v);
            }
        }
    }
}
