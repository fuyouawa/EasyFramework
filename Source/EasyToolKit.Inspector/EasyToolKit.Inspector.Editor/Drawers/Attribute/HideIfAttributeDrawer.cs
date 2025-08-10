using EasyToolKit.Core;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super + 10000)]
    public class HideIfAttributeDrawer : EasyAttributeDrawer<HideIfAttribute>
    {
        private ICodeValueResolver _conditionResolver;

        protected override void Initialize()
        {
            var targetType = Property.Parent.ValueEntry.ValueType;
            _conditionResolver = CodeValueResolver.CreateWeak(Attribute.Condition, null, targetType);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_conditionResolver.HasError(out var error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
                return;
            }

            var condition = _conditionResolver.ResolveWeak(Property.Parent.ValueEntry.WeakSmartValue);
            var value = Attribute.Value;
            var hide = Equals(condition, value);

            if (!hide)
            {
                CallNextDrawer(label);
            }
        }
    }
}
