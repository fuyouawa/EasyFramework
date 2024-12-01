using System.Collections;
using UnityEditor;
using UnityEngine;

namespace UniTool.Editor.Utilities
{
    public enum UniReorderableListThemes
    {
        UnityDefault,
        SquareLike
    }

    public class UniReorderableList : InternalReorderableList
    {
        public bool DisplayExpandButton;
        public bool DisplayCollapseButton;
        public UniReorderableListThemes Theme;

        public delegate void OnExpandCallbackDelegate();
        public delegate void OnCollapseCallbackDelegate();

        public event OnExpandCallbackDelegate OnExpandCallback;

        public event OnCollapseCallbackDelegate OnCollapseCallback;

        public UniReorderableList(IList elements,
            UniReorderableListThemes theme = UniReorderableListThemes.UnityDefault,
            bool draggable = true,
            bool displayHeader = true,
            bool displayAddButton = true,
            bool displayRemoveButton = true,
            bool displayExpandButton = true,
            bool displayCollapseButton = true)
            : base(elements, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {
            DisplayExpandButton = displayExpandButton;
            DisplayCollapseButton = displayCollapseButton;
            Theme = theme;
            Init();
        }

        public UniReorderableList(SerializedObject serializedObject,
            SerializedProperty elements,
            UniReorderableListThemes theme = UniReorderableListThemes.UnityDefault,
            bool draggable = true,
            bool displayHeader = true,
            bool displayAddButton = true,
            bool displayRemoveButton = true,
            bool displayExpandButton = true,
            bool displayCollapseButton = true)
            : base(serializedObject, elements, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {
            DisplayExpandButton = displayExpandButton;
            DisplayCollapseButton = displayCollapseButton;
            Theme = theme;
            Init();
        }

        private static class Styles
        {
            public static readonly GUIStyle Footer = "RL FooterButton";
        }

        private static readonly float BlockWidth = EditorGUIUtility.singleLineHeight;
        

        private void Init()
        {
            if (Theme == UniReorderableListThemes.SquareLike)
            {
                DisplayFooter = false;
            }

            DrawHeaderCallback += rect =>
            {
                // perform the default or overridden callback
                var rightBtnRect = new Rect(rect)
                {
                    x = rect.xMax - BlockWidth,
                    width = BlockWidth
                };
                if (Theme == UniReorderableListThemes.SquareLike)
                {
                    using (new EditorGUI.DisabledScope(!CanAdd()))
                    {
                        if (GUI.Button(rightBtnRect, UniEditorGUI.TempContent(UniEditorIcons.AddDropdown.image, "添加组件"), Styles.Footer))
                        {
                            DoAddElement(rightBtnRect);
                        }
                    }

                    rightBtnRect.x -= BlockWidth + 3;
                }

                if (DisplayCollapseButton)
                {
                    if (GUI.Button(rightBtnRect, UniEditorGUI.TempContent(UniEditorIcons.Collapse.image, "折叠所有"), Styles.Footer))
                    {
                        OnCollapseCallback?.Invoke();
                    }

                    rightBtnRect.x -= BlockWidth + 3;
                }

                if (DisplayExpandButton)
                {
                    if (GUI.Button(rightBtnRect, UniEditorGUI.TempContent(UniEditorIcons.Expand.image, "展开所有"), Styles.Footer))
                    {
                        OnExpandCallback?.Invoke();
                    }
                }
            };

            if (Theme == UniReorderableListThemes.SquareLike)
            {
                DrawElementBackgroundCallback += (rect, index, active, focused) =>
                {
                    if (index % 2 == 0)
                    {
                        EditorGUI.DrawRect(rect, new Color(0.23f, 0.23f, 0.23f));
                    }
                    else
                    {
                        EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f));
                    }
                };

                DrawElementCallback += (rect, index, active, focused) =>
                {
                    rect.y += 2;
                    var removeBtnRect = new Rect(rect)
                    {
                        x = rect.xMax
                    };
                    removeBtnRect.width = removeBtnRect.height = BlockWidth;
                    removeBtnRect.x -= EditorGUIUtility.singleLineHeight;

                    using (new EditorGUI.DisabledScope(CanRemove(index)))
                    {
                        if (GUI.Button(removeBtnRect, UniEditorGUI.TempContent(UniEditorIcons.Remove.image, "删除组件"), Styles.Footer))
                        {
                            DoRemoveElement(index);
                        }
                    }
                };
            }
        }
    }
}
