namespace EasyToolKit.Inspector.Editor
{
    public class PropertyState
    {
        public InspectorProperty Property { get; }

        private LocalPersistentContext<bool> _expanded;

        public PropertyState(InspectorProperty property)
        {
            Property = property;
        }

        public bool Expanded
        {
            get
            {
                _expanded ??= Property.GetPersistentContext("_expanded", false);

                return _expanded.Value;
            }
            set
            {
                _expanded ??= Property.GetPersistentContext("_expanded", false);

                _expanded.Value = value;
            }
        }
    }
}
