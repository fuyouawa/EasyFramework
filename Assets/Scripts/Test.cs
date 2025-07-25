using System;
using System.Collections.Generic;
using EasyToolKit.Inspector;
using EasyToolKit.Inspector.Editor;
using MyNamespace;
using Serilog;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class MyClass
{
    public int Inttt;
    public TestEnum TestEnum;
    public Transform Transform;
}

namespace MyNamespace
{
    [Serializable]

    public class TestClass
    {
        public int Int1;
        public int Int2;
        public string String3;
        public MyClass Class1;
    }
}

public enum TestEnum
{
    EnumValue1,
    EnumValue2
}

[EasyInspector]
public class Test : MonoBehaviour
{
    // [HideLabel]
    // public TestClass TestClass = new TestClass();
    // [LabelText("{{self.String1}}")]
    // public int Int1;
    public Transform Transform;
    public int Int2;
    // public int Int3;
    // public TestEnum TestEnum;
    // public string String1;
    // public Transform Transform;
    public UnityEvent TestUnityEvent;
    public List<TestClass> testList = new List<TestClass>();

    public TestClass TestClass;

    // Start is called before the first frame update
    void Start()
    {
        
        Log.Logger = new LoggerConfiguration()
            .CreateLogger();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Log.Information("asd");
        }
    }
}
