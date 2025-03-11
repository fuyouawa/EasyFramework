using System;
using System.Collections.Generic;
using EasyFramework.ToolKit;
using UnityEngine;

public partial class TestController : MonoBehaviour
{
    [MethodPickerSettings(LimitParameterTypesGetter = "GetTestLimitTypes")]
    public List<MethodPicker> TestPickers;

    public EasyEvent<float> TestEvent;

    private Type[] GetTestLimitTypes()
    {
        return new[] { typeof(string) };
    }

    private void Innnn(float fff)
    {
        Debug.Log(fff);
    }

    void Start()
    {
    }

    void Update()
    {
    }
}
