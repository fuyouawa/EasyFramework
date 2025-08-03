using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace EasyToolKit.Core.Editor
{
    public static class EasyHandleHelper
    {
        private static readonly HandleScopeStack<Color> HandlesColorStack = new HandleScopeStack<Color>();
        private static readonly HandleScopeStack<CompareFunction> HandlesZTestStack = new HandleScopeStack<CompareFunction>();

        public static void PushColor(Color color)
        {
            HandlesColorStack.Push(Handles.color);
            Handles.color = color;
        }

        public static void PopColor()
        {
            Handles.color = HandlesColorStack.Pop();
        }

        public static void PushZTest(CompareFunction compareFunction)
        {
            HandlesZTestStack.Push(Handles.zTest);
            Handles.zTest = compareFunction;
        }

        public static void PopZTest()
        {
            Handles.zTest = HandlesZTestStack.Pop();
        }
    }
}
