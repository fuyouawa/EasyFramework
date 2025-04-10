using System.Diagnostics;
using EasyFramework;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int Jj = 10;
    public float OO = 1.34f;

    void Awake()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            // .WriteTo.Async(a => a.File("Logs/log-.txt", rollingInterval: RollingInterval.Day))
            .WriteTo.UnityConsole()
            .WriteTo.GameConsole()
            .CreateLogger();
    }

    [Button]
    public void TestLog(string message)
    {
        Log.Info(message);
    }
}
