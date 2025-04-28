using System.Diagnostics;
using EasyFramework;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public class JJ
    {
        public int JK;
    }
    
    [HideLabel, InlineProperty]
    public SerializedVariant Js = new SerializedVariant("345");

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

    [GameConsoleCommand("jjbb")]
    static void Command(JJ jk)
    {

    }
}
