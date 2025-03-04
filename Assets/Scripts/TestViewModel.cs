using EasyFramework;
using EasyFramework.ToolKit;
using UnityEngine;

public class Inner
{
    public string JJ = "2344534asdfsd";
}

public class TestData
{
    public Test TestMono;
    public int Intt = 1122;
    public string Strr = "234234";
    public bool Bool = false;
    public Color Color = Color.cyan;
    public Inner Inner = new Inner();

    [SerializeField] private float ssss = 234.55f;
}


public partial class TestViewModel : MonoBehaviour
{
    public int? Value;
    public Test TestMono;

    void Start()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.UnityConsole()
            .MinimumLevel.Debug()
            .CreateLogger();

        var t = new TestData();
        t.TestMono = TestMono;
    }

    void Update()
    {
    }
}
