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

        public string OnAddedElementCallback;
        public string OnRemovedElementCallback;

        public string CustomCreateElementFunction;
        public string CustomRemoveIndexFunction;
        public string CustomRemoveElementFunction;

        public bool IsDefinedExpanded => _expanded.HasValue;

        public ListDrawerSettingsAttribute()
        {
        }
    }
}
