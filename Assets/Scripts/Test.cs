using System;
using EasyToolKit.GameConsole;
using EasyToolKit.Logging;
using UnityEngine;

[Serializable]
public class LL
{
    public int jl;
}

public class Test : MonoBehaviour
{
    public LL ll = new LL();

    // Start is called before the first frame update
    void Start()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.UnityConsole()
            .WriteTo.GameConsole()
            .CreateLogger();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Log.Info("asd");
        }
    }
}
