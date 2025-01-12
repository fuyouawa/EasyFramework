
using EasyGameFramework;
using UnityEngine;

public partial class TestViewModel : IEasyControl
{
    [SerializeField, HideInInspector]
    private EasyControlArgs _easyControlArgs = new();

    public EasyControlArgs GetEasyControlArgs() => _easyControlArgs;
    public void SetEasyControlArgs(EasyControlArgs args) => _easyControlArgs = args;

    /// <summary>
    /// <para>asdasd</para>
    /// </summary>
    [EasyBounderControl("GameObject")]
    public EasyControl GameObject;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField, EasyBounderControl("GameObject2")]
    private EasyControl _gameObject2;
}
