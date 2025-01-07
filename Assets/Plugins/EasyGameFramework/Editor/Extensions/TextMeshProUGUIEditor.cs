// using System.Reflection;
// using Pokemon.UI;
// using Sirenix.Utilities.Editor;
// using TMPro;
// using TMPro.EditorUtilities;
// using EasyFramework;
// using UnityEditor;
// using UnityEngine;
//
// namespace EasyGameFramework.Editor
// {
//     [CustomEditor(typeof(TextMeshProUGUI))]
//     [CanEditMultipleObjects]
//     public class TextMeshProUGUIEditor : UnityEditor.Editor
//     {
//         //Unity's built-in editor
//         private UnityEditor.Editor _defaultEditor;
//         private TextMeshProUGUI _textMeshProUGUI;
//
//         void OnEnable()
//         {
//             //When this inspector is created, also create the built-in inspector
//             _defaultEditor = CreateEditor(targets, typeof(TMP_EditorPanelUI));
//             _textMeshProUGUI = target as TextMeshProUGUI;
//         }
//
//         void OnDisable()
//         {
//             //When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
//             //Also, make sure to call any required methods like OnDisable
//             var disableMethod = _defaultEditor.GetType().GetMethod("OnDisable",
//                 BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
//             if (disableMethod != null)
//                 disableMethod.Invoke(_defaultEditor, null);
//             DestroyImmediate(_defaultEditor);
//         }
//
//         public override void OnInspectorGUI()
//         {
//             var mgr = _textMeshProUGUI.GetComponent<UiTextManager>();
//             if (mgr == null)
//             {
//                 SirenixEditorGUI.MessageBox("该UI文本组件没有被UITextManager管理, 您可以点击下方按钮自动添加", MessageType.Warning,
//                     EasyGUIStyles.MessageBox, true);
//                 if (GUILayout.Button("添加UITextManager"))
//                 {
//                     mgr = _textMeshProUGUI.gameObject.AddComponent<UiTextManager>();
//                     mgr.FontAssetPresetIndex = UiTextPresetsManager.Instance.DefaultFontAssetPresetIndex;
//                 }
//             }
//             else
//             {
//                 var n1 = mgr.FontAssetPreset?.Label;
//                 var n2 = mgr.TextPropertiesPreset?.Label;
//                 SirenixEditorGUI.MessageBox(
//                     $"该UI文本组件使用的预设为\n" +
//                     $"字体资产预设:{n1.DefaultIfNullOrEmpty("无")}\n" +
//                     $"Text属性预设:{n2.DefaultIfNullOrEmpty("无")}",
//                     MessageType.Info,
//                     EasyGUIStyles.MessageBox,
//                     true);
//             }
//
//             _defaultEditor.OnInspectorGUI();
//         }
//     }
// }
