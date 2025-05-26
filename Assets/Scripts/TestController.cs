using System;
using UnityEngine;
using EasyFramework.Core;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;

public class TestController : MonoBehaviour, IController
{
    #region 绑定变量
    
    [SerializeField, Binding, TitleGroup("Bindings")]
    private Transform _gameObject1;
    
    [SerializeField, Binding, TitleGroup("Bindings")]
    private Transform _gameObject2;
    
    #endregion

    void Awake()
    {
    }
    
    void Start()
    {
    }
    
    void Update()
    {
    }

    IArchitecture IBelongToArchitecture.GetArchitecture() => TestApp.Instance;
}
