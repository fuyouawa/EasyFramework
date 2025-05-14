using System;
using System.Collections.Generic;
using EasyFramework.Core;
using EasyFramework.Serialization;
using EasyFramework.ToolKit;
using EasyFramework.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
    public EasyEvent<int> Jjj;

    public int Power = 2;

    [SerializeField]
    private EasySerializationData _data;
    // public Vector2 SectionX;

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

            // seq.Append(transform.DoMove(new Vector3(10, 10, 0), 2f)     // 2s内线性缓动到(10, 10, 0)
            //     .SetRelative());                                        // 使用相对模式，结束坐标改为：起始坐标 + (10, 10, 0)
            //
            // seq.Join(Tween.Callback(() => Debug.Log("Join Callback"))); // 调用回调 (与上一个片段同时调用)
            //
            // seq.Join(transform.DoScale(Vector3.one * 0.5f, 0.5f)        // 波动效果，持续一秒钟 (与上一个片段同时调用)
            //     .SetLoopType(LoopType.Yoyo)                             // 反转方向
            //     .SetLoopCount(6));                                      // 循环6次
            //
            // // 这里会等待上一个片段完成后，再执行下一个片段
            // // 第一个Append只持续2s，而波动效果的Join会持续0.5*6=3s
            // // 所以会经过3s才会调用下面的Append
            //
            // seq.Append(transform.DoMove(new Vector3(0, 10, 0), 2f)      // 2s内移动到(0, 10, 0)
            //     .SetEase(Ease.InSine()));                               // 使用Sine曲线缓动效果
            //
            // seq.Append(transform.DoMove(new Vector3(0, 0, 0), 4f)       // 线性缓动到(0, 0, 0)
            //     .SetSpeedBased());                                      // 将持续时间变成速度，也就是4m/s

            seq.Append(transform.DoMove(new Vector3(10, 10, 0), 3f)     // 3s内移动到(10, 10, 0)
                .SetRelative()
                .SetEase(Ease.InExponential(Power))                     // 使用指数函数缓动
                .SetEffect(Effect.Bezier()                              // 使用二次贝塞尔曲线效果
                   .SetControlPoint(new Vector3(-5, 5, 0))
                   .SetControlPointRelative(BezierControlPointRelativeTo.StartPoint)));
            seq.Append(Tween.Callback(() => Debug.Log("Finish All!")));
        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            var val = new MyClass();

            var data = new EasySerializationData(EasyDataFormat.Json);

            EasySerialize.To(val, ref data);
            Debug.Log(data.StringData);
            EasySerialize.From<MyClass>(ref data);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.DoMove(new Vector3(10, 10, 0), 2f) // 2s内线性缓动到(10, 10, 0)
                .SetRelative();

            transform.DoScale(Vector3.one * 0.5f, 0.5f) // 波动效果，持续一秒钟 (与上一个片段同时调用)
                .SetLoopType(LoopType.Yoyo)             // 反转方向
                .SetInfiniteLoop()
                .SetId("TTTT");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Tween.Kill("TTTT");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            var val = new MyClass();
            
            var data = new EasySerializationData(EasyDataFormat.Binary);
            
            int count = 10000;
            var begin = DateTime.Now;
            
            for (int i = 0; i < count; i++)
            {
                EasySerialize.To(val, ref data);
            }
            
            var end = DateTime.Now;
            var diff = end - begin;
            Debug.Log($"Serialize {count} element use {diff.TotalSeconds} time");
            Debug.Log($"Serialize data length: {data.BinaryData.Length}");
            
            
            begin = DateTime.Now;
            for (int i = 0; i < count; i++)
            {
                EasySerialize.From<MyClass>(ref data);
            }
            
            end = DateTime.Now;
            diff = end - begin;
            Debug.Log($"Deserialize {count} element use {diff.TotalSeconds} time");

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            var val = new MyClass();
            
            var data = new EasySerializationData(EasyDataFormat.Json);
            
            int count = 10000;
            var begin = DateTime.Now;
            
            for (int i = 0; i < count; i++)
            {
                EasySerialize.To(val, ref data);
            }
            
            var end = DateTime.Now;
            var diff = end - begin;
            Debug.Log($"Serialize {count} element use {diff.TotalSeconds} time");
            Debug.Log($"Serialize data length: {data.StringData.Length} | data: {data.StringData}");
            
            
            begin = DateTime.Now;
            for (int i = 0; i < count; i++)
            {
                var cls = EasySerialize.From<MyClass>(ref data);
            }
            
            end = DateTime.Now;
            diff = end - begin;
            Debug.Log($"Deserialize {count} element use {diff.TotalSeconds} time");

            _data = data;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            var val = new MyClass();
            
            int count = 10000;
            var begin = DateTime.Now;

            byte[] bytes = {};
            for (int i = 0; i < count; i++)
            {
                bytes = SerializationUtility.SerializeValue(val, DataFormat.Binary, out _);
            }
            
            var end = DateTime.Now;
            var diff = end - begin;
            Debug.Log($"Serialize {count} element use {diff.TotalSeconds} time");
            Debug.Log($"Serialize data length: {bytes.Length}");
            
            begin = DateTime.Now;
            for (int i = 0; i < count; i++)
            {
                SerializationUtility.DeserializeValue<MyClass>(bytes, DataFormat.Binary);
            }
            
            end = DateTime.Now;
            diff = end - begin;
            Debug.Log($"Deserialize {count} element use {diff.TotalSeconds} time");
        }
    }

    // public void TestLog(InputField input)
    // {
    //     Log.Info(input.text);
    // }
    //

    public enum TestEnum
    {
        JJB,
        KKAAm,
        AS
    }

    class MyClass
    {
        public List<MyClass2> bbss = new List<MyClass2>()
        {
            new MyClass2(),
            new MyClass2(),
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

        public TestEnum JJAS = TestEnum.JJB;
        public TestEnum JJAAS = TestEnum.KKAAm;
        public TestEnum JJAS3 = TestEnum.AS;

        public Type KKAsq = typeof(MyClass);
        public Type KKAs = typeof(MyClass);

        public class MyClass2
        {
            public int jks = 100;
            public string sss = "234545";
            public float asdzx = 34.5f;
        }

        public MyClass2 cc2 = new MyClass2();
        public MyClass2 cc2a = new MyClass2();
        public MyClass2 cc2c = new MyClass2();

        public MyClass2[] cc3 = new MyClass2[]
        {
            new MyClass2(),
            new MyClass2(),
            new MyClass2(),
            new MyClass2(),
        };
    }
    //
    // [GameConsoleCommand("jjbb")]
    // static void Command(JJ jk)
    // {
    // }
    //
    // static ProfilerMarker s_profilerMarker = new ProfilerMarker("Serialize");
    //
    // [Button]
    // public void TestLLLL()
    // {
    //     var val = new MyClass();
    //
    //     var data = new EasySerializationData(EasyDataFormat.Json);
    //     
    //     EasySerialize.To(val, ref data);
    //     EasySerialize.From<MyClass>(ref data);
    //
    //     // int count = 10000;
    //     // var begin = DateTime.Now;
    //     //
    //     // s_profilerMarker.Begin(this);
    //     // for (int i = 0; i < count; i++)
    //     // {
    //     //     EasySerialize.To(val, ref data);
    //     // }
    //     //
    //     // s_profilerMarker.End();
    //     //
    //     // var end = DateTime.Now;
    //     // var diff = end - begin;
    //     // Debug.Log($"Serialize {count} element use {diff.TotalSeconds} time");
    //     // Debug.Log($"Serialize data: {data.StringData}");
    //     //
    //     //
    //     // begin = DateTime.Now;
    //     // for (int i = 0; i < count; i++)
    //     // {
    //     //     EasySerialize.From<MyClass>(ref data);
    //     // }
    //     //
    //     // end = DateTime.Now;
    //     // diff = end - begin;
    //     // Debug.Log($"Deserialize {count} element use {diff.TotalSeconds} time");
    //
    //     // EasySerialize.To(val, ref data);
    //     // var cls = EasySerialize.From<MyClass>(ref data);
    //     // return;
    // }
}
