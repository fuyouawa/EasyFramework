using System;
using System.Collections.Generic;
using EasyFramework;
using EasyFramework.ToolKit;
using UnityEngine;



public partial class TestViewModel : MonoBehaviour
{
    public int? Value;

	void Start()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.UnityConsole()
            .MinimumLevel.Debug()
            .CreateLogger();

        Log.Debug("114514");
    }
	
	void Update()
	{
	}
}
