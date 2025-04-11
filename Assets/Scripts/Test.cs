using System.Diagnostics;
using EasyFramework;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    void Awake()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            // .WriteTo.Async(a => a.File("Logs/log-.txt", rollingInterval: RollingInterval.Day))
            .WriteTo.UnityConsole()
            .WriteTo.GameConsole()
            .CreateLogger();
    }

    public void TestLog(InputField input)
    {
        Log.Info(input.text);
    }
}
