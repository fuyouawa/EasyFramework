using System.Diagnostics;
using System;

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class ListDrawerSettingsAttribute : Attribute
    {
        private bool? _expanded;

        public bool Expanded
        {
            get => _expanded ?? throw new InvalidOperationException();
            set => _expanded = value;
        }

        public bool HideAddButton { get; set; }
        public bool HideRemoveButton { get; set; }

        public string OnAddedElementCallback { get; set; }
        public string OnRemovedElementCallback { get; set; }

        public string CustomCreateElementFunction { get; set; }
        public string CustomRemoveIndexFunction { get; set; }
        public string CustomRemoveElementFunction { get; set; }

        public bool IsDefinedExpanded => _expanded.HasValue;

        public ListDrawerSettingsAttribute()
        {
        }
    }
}
