using System;
using System.Collections.Generic;
using EasyFramework;
using UnityEngine;



public partial class TestViewModel : MonoBehaviour
{
    
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
