
using EasyGameFramework;
using UnityEngine;

public partial class TestViewModel
{

    [EasyBounderControl("GameObject")]
    public EasyControl GameObject;

    [EasyBounderControl("GameObject2")]
    public EasyControl GameObject2;

#if UNITY_EDITOR
    /// <summary>
    /// <para>EasyControl的编辑器参数</para>
    /// <para>（不要在代码中使用，仅在编辑器中有效！）</para>
    /// </summary>
    [SerializeField]
    private EasyControlEditorArgs _easyControlEditorArgs = new();
#endif
}