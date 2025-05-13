using UnityEditor;
using UnityEditor.UI;

namespace EasyFramework.ToolKit.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(EasyToggle))]
    public class EasyToggleEditor : SelectableEditor
    {
        private SerializedProperty _toggleTransitionProperty;
        private SerializedProperty _graphicProperty;
        private SerializedProperty _groupProperty;
        private SerializedProperty _onValueChangedProperty;
        private SerializedProperty _isOnProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            _toggleTransitionProperty = serializedObject.FindProperty("ToggleTransition");
            _graphicProperty = serializedObject.FindProperty("Graphic");
            _groupProperty = serializedObject.FindProperty("_group");
            _onValueChangedProperty = serializedObject.FindProperty("OnValueChanged");
            _isOnProperty = serializedObject.FindProperty("_isOn");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(_toggleTransitionProperty);
            EditorGUILayout.PropertyField(_graphicProperty);
            EditorGUILayout.PropertyField(_groupProperty);
            EditorGUILayout.PropertyField(_onValueChangedProperty);
            EditorGUILayout.PropertyField(_isOnProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
