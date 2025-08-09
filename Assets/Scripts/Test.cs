using EasyToolKit.Inspector;
using Serilog;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[EasyInspector]
public class Test : MonoBehaviour
{
    public int i;
    [InlineEditor]
    public Test2 test2;
}