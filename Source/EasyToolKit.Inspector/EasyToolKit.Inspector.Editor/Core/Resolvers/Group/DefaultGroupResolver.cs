using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultGroupResolver : GroupResolver
    {
        private InspectorProperty[] _groupProperties;

        public override InspectorProperty[] GetGroupProperties()
        {
            if (_groupProperties != null)
            {
                return _groupProperties;
            }

            var beginGroupAttribute = (BeginGroupAttribute)Property.GetAttributes().FirstOrDefault(attr => attr is BeginGroupAttribute);
            if (beginGroupAttribute == null)
            {
                _groupProperties = new InspectorProperty[] { };
                return _groupProperties;
            }

            var parent = Property.Parent;

            int beginIndex = 0;
            for (; beginIndex < parent.Children.Count; beginIndex++)
            {
                if (parent.Children[beginIndex] == Property)
                {
                    break;
                }
            }

            if (beginIndex >= parent.Children.Count - 1)
            {
                _groupProperties = new InspectorProperty[] { };
                return _groupProperties;
            }
            

            var groupName = beginGroupAttribute.GroupName;
            var beginGroupAttributeType = beginGroupAttribute.GetType();
            var endGroupAttributeType =
                InspectorAttributeUtility.GetCorrespondGroupAttributeType(beginGroupAttribute.GetType());

            var groupPropertyList = new List<InspectorProperty>();
            var subGroupPropertyStack = new Stack<InspectorProperty>();
            for (int i = beginIndex + 1; i < parent.Children.Count; i++)
            {
                var child = parent.Children[i];

                var childBeginGroupAttribute = (BeginGroupAttribute)Property.GetAttributes().FirstOrDefault(attr => attr.GetType() == beginGroupAttributeType);
                if (childBeginGroupAttribute != null)
                {
                    var childGroupName = childBeginGroupAttribute.GroupName;
                    bool isSubGroup = groupName.IsNotNullOrEmpty() &&
                                      childGroupName.IsNotNullOrEmpty() &&
                                      childGroupName.StartsWith(groupName) &&
                                      childGroupName[groupName.Length] == '/';

                    if (isSubGroup)
                    {
                        subGroupPropertyStack.Push(child);
                    }
                    else
                    {
                        break;
                    }
                }

                var childEndGroupAttribute = (EndGroupAttribute)Property.GetAttributes().FirstOrDefault(attr => attr.GetType() == endGroupAttributeType);
                if (childEndGroupAttribute != null)
                {
                    if (subGroupPropertyStack.Count > 0)
                    {
                        subGroupPropertyStack.Pop();
                    }
                    else
                    {
                        break;
                    }
                }

                groupPropertyList.Add(child);
            }

            _groupProperties = groupPropertyList.ToArray();
            return _groupProperties;
        }
    }
}
