using EasyFramework;
using EasyFramework.ToolKit;
using UnityEngine;

public partial class TestViewModel : MonoBehaviour
{
    void Start()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.UnityConsole()
            .MinimumLevel.Debug()
            .CreateLogger();
    }

    void Update()
    {
    }
}

