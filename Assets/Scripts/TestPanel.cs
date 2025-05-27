using System;
using EasyFramework.UIKit;
using EasyFramework.Core;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class TestPanel : UIPanel, IController
{
    #region 绑定变量
    
    [SerializeField, Binding, TitleGroup("Binding")]
    private TMP_Dropdown _dropdown;
    
    [SerializeField, Binding, TitleGroup("Binding")]
    private TMP_InputField _testInput2;
    
    [SerializeField, Binding, TitleGroup("Binding")]
    private TMP_InputField _testInput;
    
    #endregion

    protected override void OnOpen(IPanelData panelData)
    {
    }
    
    
    protected override void OnClose()
    {
    }

    IArchitecture IBelongToArchitecture.GetArchitecture() => TestApp.Instance;
}
