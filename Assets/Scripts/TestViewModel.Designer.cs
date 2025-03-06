using EasyFramework.ToolKit;
using UnityEngine;

public partial class TestViewModel : IViewController
{
    [AutoBinding, SerializeField] public Transform Dsfgdfg;
    [AutoBinding, SerializeField] public Transform Dsfgdfgs3r;
    
    [SerializeField] private ViewControllerConfig _viewControllerConfig;

    ViewControllerConfig IViewController.Config
    {
        get => _viewControllerConfig;
        set => _viewControllerConfig = value;
    }
}
