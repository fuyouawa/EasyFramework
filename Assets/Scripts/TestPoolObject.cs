using System;
using System.Collections;
using System.Collections.Generic;
using EasyFramework.Core;
using EasyFramework.ToolKit;
using UnityEngine;

public class OOO : AbstractPooledObject
{
    public int kkn;

    protected override void OnSpawn()
    {
        Debug.Log("OnSpawn");
    }

    protected override void OnRecycle()
    {
        Debug.Log("OnRecycle");
    }
}

public class TestPoolObject : MonoBehaviour
{
    private GameObject o111;
    // Start is called before the first frame update
    void Start()
    {
        o111 = new GameObject("IIJbsas");
        o111.AddComponent<AudioSource>();
        o111.AddComponent<Rigidbody>();
        o111.AddComponent<BoxCollider>();
        o111.AddComponent<ParticleSystem>();
        o111.AddComponent<Feedbacks>();
        o111.AddComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            var pool = ObjectPoolManager.Instance.TryGetOrAllocatePool<OOO>("DEFAULT");
            var o = pool.TrySpawn<OOO>();
            o.kkn = 1;
            var o1 = pool.TrySpawn<OOO>();
            o1.kkn = 2;
            var o2 = pool.TrySpawn<OOO>();
            o2.kkn = 3;

            pool.TryRecycle(o);
            pool.TryRecycle(o1);
            pool.TryRecycle(o2);

            o = pool.TrySpawn<OOO>();
            o.kkn = 4;
            o1 = pool.TrySpawn<OOO>();
            o1.kkn = 5;
            o2 = pool.TrySpawn<OOO>();
            o2.kkn = 6;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {

            var pool = UnityObjectPoolManager.Instance.TryGetOrAllocatePool<AudioSource>("JJB", o111);
            var o = pool.TrySpawn<AudioSource>();
            var o1 = pool.TrySpawn<AudioSource>();
            var o2 = pool.TrySpawn<AudioSource>();
        }

        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     var count = 1000000;
        //     var begin = DateTime.Now;
        //
        //     for (int i = 0; i < count; i++)
        //     {
        //         ComponentSearcher.Instance.Get<AudioSource>(o111);
        //     }
        //
        //     var end = DateTime.Now;
        //     var diff = end - begin;
        //     Debug.Log($"Get {count} element use {diff.TotalSeconds} time");
        //
        //     begin = DateTime.Now;
        //
        //     for (int i = 0; i < count; i++)
        //     {
        //         o111.GetComponent<AudioSource>();
        //     }
        //
        //     end = DateTime.Now;
        //     diff = end - begin;
        //     Debug.Log($"Get {count} element use {diff.TotalSeconds} time");
        // }
    }
}
