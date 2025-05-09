using System.Collections.Generic;
using EasyFramework.Serialization;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public class JJ
    {
        public int JK;
    }

    [HideLabel, InlineProperty]
    public SerializedVariant Js = new SerializedVariant("345");

    void Awake()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            // .WriteTo.Async(a => a.File("Logs/log-.txt", rollingInterval: RollingInterval.Day))
            .WriteTo.UnityConsole()
            .WriteTo.GameConsole()
            .CreateLogger();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            var seq = Tween.Sequence();
            seq.Append(transform.MovePos(new Vector3(10, 10, 0), 4f));      // 4s内线性缓动到(10, 10, 0)
            seq.Join(transform.PlayLocalScale(Vector3.one * 0.5f, 1f)
                .SetLoop(6, LoopType.Yoyo));

            seq.Append(transform.MovePos(new Vector3(0, 10, 0), 2f)
                .SetEaseMode(EaseMode.InSine));                             // 2s内Sine曲线缓动到(0, 10, 0)

            seq.Append(transform.MovePos(new Vector3(0, 0, 0), 2f)
                .SetDuration(DurationMode.Speed));                          // 2m/s的速度线性缓动到(0, 0, 0)
        }
    }

    public void TestLog(InputField input)
    {
        Log.Info(input.text);
    }

    class MyClass
    {
        public List<MyClass2> bbss = new List<MyClass2>()
        {
            new MyClass2(),
            new MyClass2(),
            new MyClass2(),
            new MyClass2(),
        };

        public int jks = 100;
        public string sss = "234545";
        public string asdf = "$65dfg";
        public float asdzx = 34.5f;
        public double sadxz = 2345.56;

        public class MyClass2
        {
            public int jks = 100;
            public string sss = "234545";
            public float asdzx = 34.5f;
        }

        public MyClass2 cc2 = new MyClass2();

        public MyClass2[] cc3 = new MyClass2[]
        {
            new MyClass2(),
            new MyClass2(),
            new MyClass2(),
        };
    }

    [GameConsoleCommand("jjbb")]
    static void Command(JJ jk)
    {
    }

    static ProfilerMarker s_profilerMarker = new ProfilerMarker("Serialize");

    [Button]
    public void TestLLLL()
    {
        var val = new MyClass();

        var data = new EasySerializationData(EasyDataFormat.Json);
        
        EasySerialize.To(val, ref data);
        EasySerialize.From<MyClass>(ref data);

        // int count = 10000;
        // var begin = DateTime.Now;
        //
        // s_profilerMarker.Begin(this);
        // for (int i = 0; i < count; i++)
        // {
        //     EasySerialize.To(val, ref data);
        // }
        //
        // s_profilerMarker.End();
        //
        // var end = DateTime.Now;
        // var diff = end - begin;
        // Debug.Log($"Serialize {count} element use {diff.TotalSeconds} time");
        // Debug.Log($"Serialize data: {data.StringData}");
        //
        //
        // begin = DateTime.Now;
        // for (int i = 0; i < count; i++)
        // {
        //     EasySerialize.From<MyClass>(ref data);
        // }
        //
        // end = DateTime.Now;
        // diff = end - begin;
        // Debug.Log($"Deserialize {count} element use {diff.TotalSeconds} time");

        // EasySerialize.To(val, ref data);
        // var cls = EasySerialize.From<MyClass>(ref data);
        // return;
    }
}
