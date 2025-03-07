using EasyFramework.ToolKit;
using UnityEngine;

public partial class TestController : IViewController
{
    [AutoBinding, SerializeField] public Transform Dsfgdfg;
    [AutoBinding, SerializeField] public GameObject Dsfgdfgs3r;
    [AutoBinding, SerializeField] private SpriteRenderer _circle;
    
    [SerializeField] private ViewControllerConfig _viewControllerConfig;

    ViewControllerConfig IViewController.Config
    {
        get => _viewControllerConfig;
        set => _viewControllerConfig = value;
    }
}
