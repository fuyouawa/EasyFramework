using System;
using TMPro;
using UnityEngine;

namespace EasyFramework.UIKit
{
    [Serializable]
    public class TextStyle
    {
        [SerializeField] private string _name;
        [SerializeField] private TMP_FontAsset _fontAsset;
        [SerializeField] private Material _fontMaterial;
        [SerializeField] private float _fontSize = 26;
        [SerializeField] private Color _fontColor = Color.white;

        public string Name => _name;
        public TMP_FontAsset FontAsset => _fontAsset;
        public Material FontMaterial => _fontMaterial;
        public float FontSize => _fontSize;
        public Color FontColor => _fontColor;
    }
}
