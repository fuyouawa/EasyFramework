using System;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class EasyDrawer
    {
        public bool SkipWhenDrawing { get; set; }
        public InspectorProperty Property { get; private set; }

        public static EasyDrawer Create([NotNull] Type drawerType, [NotNull] InspectorProperty property)
        {
            if (drawerType == null) throw new ArgumentNullException(nameof(drawerType));
            if (property == null) throw new ArgumentNullException(nameof(property));

            if (!typeof(EasyDrawer).IsAssignableFrom(drawerType))
            {
                throw new ArgumentException(nameof(drawerType));
            }

            var drawer = drawerType.CreateInstance<EasyDrawer>();
            drawer.Property = property;
            drawer.Initialize();
            return drawer;
        }

        protected virtual void Initialize()
        {
        }

        internal void DrawProperty(GUIContent label)
        {
            OnDrawProperty(label);
        }

        public virtual bool CanDrawProperty(InspectorProperty property)
        {
            return true;
        }
        
        protected bool CallNextDrawer(GUIContent label)
        {
            var chain = Property.GetDrawerChain();
            if (chain.MoveNext())
            {
                chain.Current.DrawProperty(label);
                return true;
            }
            return false;
        }

        protected virtual void OnDrawProperty(GUIContent label)
        {
        }
    }
}
