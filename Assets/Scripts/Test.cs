using System;
using EasyToolKit.GameConsole;
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

public class Test : MonoBehaviour
{
    public int jl;
    public int i;
    public int j;
    public string jb;
    public LL ll = new LL();

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
public class TestEditor : EasyEditor
{

}

#endif
