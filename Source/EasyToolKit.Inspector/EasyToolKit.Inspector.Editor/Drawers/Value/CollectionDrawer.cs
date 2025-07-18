using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditorInternal;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    class CollectionElementContext
    {
        public readonly DelayedGUIDrawer DelayedGUIDrawer = new DelayedGUIDrawer();
        public float Height;
    }

    [DrawerPriority(DrawerPriorityLevel.Value + 10)]
    public class CollectionDrawer<T> : EasyValueDrawer<T>
    {

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return property.ChildrenResolver is InspectorCollectionResolver<T>;
        }

        private ReorderableList _reorderableList;
        private InspectorCollectionResolver<T> _collectionResolver;
        private readonly List<CollectionElementContext> _elementContexts = new List<CollectionElementContext>();

        protected override void Initialize()
        {
            _reorderableList = new ReorderableList(
                Property.Tree.SerializedObject,
                Property.Tree.SerializedObject.FindProperty(Property.Info.PropertyPath),
                draggable:true,
                displayHeader:true,
                displayAddButton:true,
                displayRemoveButton:true);

            _reorderableList.drawHeaderCallback = OnDrawHeader;
            _reorderableList.drawElementCallback = OnDrawElement;
            _reorderableList.elementHeightCallback = GetElementHeight;
            _collectionResolver = (InspectorCollectionResolver<T>)Property.ChildrenResolver;
        }

        private float GetElementHeight(int index)
        {
            if (index < _elementContexts.Count)
            {
                return _elementContexts[index].Height;
            }
            return _reorderableList.elementHeight;
        }

        private void OnDrawHeader(Rect rect)
        {
            GUI.Label(rect, Property.Label);
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var child = Property.Children[index];
            
            for (int i = 0; i <= index - _elementContexts.Count; i++)
            {
                _elementContexts.Add(new CollectionElementContext());
            }

            var context = _elementContexts[index];

            context.DelayedGUIDrawer.Begin(rect.size);

            EasyGUIHelper.CalculateCurrentLayoutHeight();
            var beginHeight = EasyGUIHelper.GetCurrentLayoutMinHeight();

            child.Draw(GUIContent.none);

            EasyGUIHelper.CalculateCurrentLayoutHeight();
            var endHeight = EasyGUIHelper.GetCurrentLayoutMaxHeight();
            context.Height = endHeight - beginHeight;

            context.DelayedGUIDrawer.End();

            Property.Tree.QueueCallback(() =>
            {
                context.DelayedGUIDrawer.Draw(rect.position);
            });
        }

        protected override void OnDrawProperty(GUIContent label)
        {
            _reorderableList.DoLayoutList();
        }
    }
}
