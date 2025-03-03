using EasyFramework;
using EasyFramework.Serialization;
using EasyFramework.ToolKit;
using UnityEngine;

public class Inner
{
    public string JJ = "2344534asdfsd";
}

public class Test
{
    public int Intt = 1122;
    public string Strr = "234234";
    public Inner Inner = new Inner();

    [SerializeField]
    private float ssss = 234.55f;
}

public partial class TestViewModel : MonoBehaviour
{
    public int? Value;

	void Start()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.UnityConsole()
            .MinimumLevel.Debug()
            .CreateLogger();

        var t = new Test();
        var b = EasySerialize.ToJson(t);
        Debug.Log(b);
        var ss = EasySerialize.FromJson<Test>(b);
    }
	
	void Update()
	{
	}
}
