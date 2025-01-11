
using EasyGameFramework;
using UnityEngine;
using EasyGameFramework;

public partial class TestViewModel : IEasyControl
{
    [SerializeField, HideInInspector]
    private EasyControlArgs _easyControlArgs = new();

    /// <summary>
    /// 
    /// </summary>
    [EasyBounderControl]
    public EasyControl GameObject;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField, EasyBounderControl]
    private EasyControl GameObject2;
}

#if UNITY_EDITOR
namespace Editor
{
    using UnityEditor;
    using EasyGameFramework.Editor;

    [CustomEditor(typeof(TestViewModel))]
    public class TestViewModelEditor : EasyControlEditorBase
    {
    }
}
#endif
