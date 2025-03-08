using EasyFramework.ToolKit;
using UnityEngine;
using Sirenix.OdinInspector;

public partial class TestController : IViewController
{
    [TitleGroupEx("test"), AutoBinding, SerializeField]
    public Transform Dsfgdfg;
    [TitleGroupEx("test"), AutoBinding, SerializeField]
    public GameObject Dsfgdfgs3r;
    [TitleGroupEx("test"), AutoBinding, SerializeField]
    private SpriteRenderer _circle;
    
    [SerializeField] private ViewControllerConfig _viewControllerConfig;

    ViewControllerConfig IViewController.Config
    {
        get => _viewControllerConfig;
        set => _viewControllerConfig = value;
    }
}
