using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using UnityEditor;
using UnityEngine;

[Serializable]
public class LL
{
    public int jl;
}

public class Test : MonoBehaviour
{
    public LL ll = new LL();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
        }
    }
}
