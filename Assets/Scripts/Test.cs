using System;
using EasyToolKit.GameConsole;
using EasyToolKit.Inspector;
using EasyToolKit.Inspector.Editor;
using MyNamespace;
using Serilog;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyNamespace
{
    [Serializable]

    public class LL
    {
        public int jl;
        public int jls;
        public string jbl;
    }
}

public enum Enn
{
    kknn,
    assd
}

public class Test : MonoBehaviour
{
    public LL ll = new LL();
    [LabelText("{{self.jb}}")]
    public int jl;
    public int i;
    public int j;
    public Enn ssaf;
    public string jb;

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
