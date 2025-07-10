using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Inspector.Editor;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            InspectorConfig.Instance.UpdateEditors();
        }
    }
}
