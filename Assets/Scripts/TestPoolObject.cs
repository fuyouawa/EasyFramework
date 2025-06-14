using System;
using System.Collections;
using System.Collections.Generic;
using EasyFramework.Core;
using EasyFramework.ToolKit;
using UnityEngine;

public class OOO : IPoolCallbackReceiver
{
    public int kkn;

    void IPoolCallbackReceiver.OnRent(IObjectPool owningPool)
    {
        Debug.Log($"OnSpawn: {kkn}");
    }

    void IPoolCallbackReceiver.OnRelease(IObjectPool owningPool)
    {
        Debug.Log($"OnRecycle: {kkn}");
    }
}

public class Bullet : MonoBehaviour {}

public class TestPoolObject : MonoBehaviour
{
    private GameObject o111;
    // Start is called before the first frame update
    void Start()
    {
        o111 = new GameObject("Bullet");
        o111.AddComponent<Bullet>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            var pool = ObjectPoolManager.Instance.GetOrCreatePool<OOO>("DEFAULT");
            var o = pool.Rent<OOO>();
            o.kkn = 1;
            var o1 = pool.Rent<OOO>();
            o1.kkn = 2;
            var o2 = pool.Rent<OOO>();
            o2.kkn = 3;

            pool.Release(o);
            pool.Release(o1);
            pool.Release(o2);

            o = pool.Rent<OOO>();
            o.kkn = 4;
            o1 = pool.Rent<OOO>();
            o1.kkn = 5;
            o2 = pool.Rent<OOO>();
            o2.kkn = 6;
            
            pool.Release(o);
            pool.Release(o1);
            pool.Release(o2);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            var pool = GameObjectPoolManager.Instance.GetOrCreatePool<Bullet>("玩家子弹", o111);

            var o = pool.Rent<Bullet>();
            var o1 = pool.Rent<Bullet>();
            var o2 = pool.Rent<Bullet>();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            var pool = (GameObjectPool)GameObjectPoolManager.Instance.GetOrCreatePool<Bullet>("怪物A子弹", o111);
            pool.DefaultTimeToRecycleObject = 2f;

            var o = pool.Rent<Bullet>();
            var o1 = pool.Rent<Bullet>();
            var o2 = pool.Rent<Bullet>();
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
