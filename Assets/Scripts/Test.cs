using System;
using EasyToolKit.GameConsole;
using EasyToolKit.Inspector;
using EasyToolKit.Inspector.Editor;
using MyNamespace;
using Serilog;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyNamespace
{
    [Serializable]

    public class TestClass
    {
        public int Int1;
        public int Int2;
        public string String3;
    }
}

public enum TestEnum
{
    EnumValue1,
    EnumValue2
}

public class Test : MonoBehaviour
{
    [HideLabel]
    public TestClass TestClass = new TestClass();
    [LabelText("{{self.String1}}")]
    public int Int1;
    public int Int2;
    public int Int3;
    public TestEnum TestEnum;
    public string String1;
    public Transform Transform;
    public UnityEvent UnityEvent;


    // Start is called before the first frame update
    void Start()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.GameConsole()
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


#if UNITY_EDITOR
[CustomEditor(typeof(Test))]
[CanEditMultipleObjects]
public class TestEditor : EasyEditor
{

}

#endif
