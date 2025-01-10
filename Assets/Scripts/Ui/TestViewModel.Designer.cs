
using UnityEngine;
using EasyGameFramework;

public partial class TestViewModel : IEasyViewModel
{
    [SerializeField, HideInInspector]
    private EasyViewModelArgs _easyViewModelArgs = new();
    /// <summary>
    /// 
    /// </summary>
    public Transform GameObject;
    /// <summary>
    /// 
    /// </summary>
    public Transform GameObject2;
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
