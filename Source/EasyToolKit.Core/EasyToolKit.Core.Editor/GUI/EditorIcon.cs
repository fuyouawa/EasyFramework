using System;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public class EditorIcon
    {
        private readonly byte[] _data;
        private Texture2D _rawTexture;
        private Texture2D _activeTexture;
        private Texture2D _inactiveTexture;
        private Texture2D _highlightedTexture;
        private GUIContent _activeContent;
        private GUIContent _inactiveContent;
        private GUIContent _highlightedContent;

        public Texture2D RawTexture
        {
            get
            {
                if (_rawTexture == null)
                {
                    _rawTexture = TextureUtility.LoadImage(_data);
                }
                return _rawTexture;
            }
        }

        public Texture2D ActiveTexture
        {
            get
            {
                if (_activeTexture == null)
                {
                    _activeTexture = EditorIconUtility.RenderIcon(RawTexture, EasyGUIStyles.ActiveColor);
                }
                return _activeTexture;
            }
        }

        public Texture2D InactiveTexture
        {
            get
            {
                if (_inactiveTexture == null)
                {
                    _inactiveTexture = EditorIconUtility.RenderIcon(RawTexture, EasyGUIStyles.InactiveColor);
                }
                return _inactiveTexture;
            }
        }

        public Texture2D HighlightedTexture
        {
            get
            {
                if (_highlightedTexture == null)
                {
                    _highlightedTexture = EditorIconUtility.RenderIcon(RawTexture, EasyGUIStyles.HighlightedColor);
                }
                return _highlightedTexture;
            }
        }

        public GUIContent ActiveContent
        {
            get
            {
                if (_activeContent == null)
                {
                    _activeContent = new GUIContent(ActiveTexture);
                }

                return _activeContent;
            }
        }

        public GUIContent InactiveContent
        {
            get
            {
                if (_inactiveContent == null)
                {
                    _inactiveContent = new GUIContent(InactiveTexture);
                }

                return _inactiveContent;
            }
        }

        public GUIContent HighlightedContent
        {
            get
            {
                if (_highlightedContent == null)
                {
                    _highlightedContent = new GUIContent(HighlightedContent);
                }

                return _highlightedContent;
            }
        }

        public EditorIcon(string base64)
        {
            _data = Convert.FromBase64String(base64);
        }

        public EditorIcon(byte[] data)
        {
            _data = data;
        }

        public void Draw(Rect rect)
        {
            if (Event.current.type != EventType.Repaint) return;

            Texture2D texture;
            if (!GUI.enabled)
            {
                texture = InactiveTexture;
            }
            else if (rect.Contains(Event.current.mousePosition))
            {
                EasyGUIHelper.RequestRepaint();
                texture = HighlightedTexture;
            }
            else
            {
                texture = ActiveTexture;
            }

            GUI.DrawTexture(rect, texture);
        }
    }
}
