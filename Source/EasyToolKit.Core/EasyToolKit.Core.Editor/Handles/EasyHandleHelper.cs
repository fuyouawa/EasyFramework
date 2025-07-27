using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public static class EasyHandleHelper
    {
        private static readonly HandleScopeStack<Color> HandlesColorStack = new HandleScopeStack<Color>();

        public static void PushColor(Color color)
        {
            HandlesColorStack.Push(Handles.color);
            Handles.color = color;
        }

        public static void PopColor()
        {
            Handles.color = HandlesColorStack.Pop();
        }
    }
}
