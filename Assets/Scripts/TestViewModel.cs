using EasyFramework;
using EasyFramework.Serialization;
using EasyFramework.ToolKit;
using UnityEngine;

public class Test
{
    public int Intt = 1122;
    public string Strr = "234234";

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
        var b = EasySerialize.ToBinary(t);
        var ss = EasySerialize.FromBinary<Test>(b);
    }
	
	void Update()
	{
	}
}
