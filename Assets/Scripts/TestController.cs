using System;
using EasyFramework.UIKit;
using EasyFramework.Core;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestController : UIPanel, IController
{
    #region 绑定变量
    
    [SerializeField, Binding, TitleGroup("Binding")]
    private Transform _gameObject1;
    
    [SerializeField, Binding, TitleGroup("Binding")]
    private Transform _gameObject2;
    
    #endregion

    protected override void OnOpen(IPanelData panelData)
    {
    }
    
    
    protected override void OnClose()
    {
    }

    IArchitecture IBelongToArchitecture.GetArchitecture() => TestApp.Instance;
}
