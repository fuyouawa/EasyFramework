using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyFramework.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.UIKit
{
    [ScriptableObjectSingletonAssetPath("Resources")]
    public class TextStyleLibrary : ScriptableObjectSingleton<TextStyleLibrary>
    {
        [SerializeField]
        private List<TextStyle> _styles = new List<TextStyle>();

        [ValueDropdown(nameof(GetStyleNamesDropdown))]
        [SerializeField]
        private string _defaultStyleName;

        public IReadOnlyList<TextStyle> Styles => _styles;

        private IEnumerable GetStyleNamesDropdown()
        {
            return _styles.Select(style => style.Name);
        }

        public TextStyle GetStyleByName(string styleName)
        {
            if (styleName.IsNullOrEmpty())
            {
                return null;
            }
            return _styles.FirstOrDefault(style => style.Name == styleName);
        }

        public TextStyle GetDefaultStyle()
        {
            return GetStyleByName(_defaultStyleName);
        }
    }
}
