using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EasyFramework;
using EasyFramework.ToolKit;
using UnityEngine;



public partial class TestViewModel : MonoBehaviour
{
    public int? Value;

	void Start()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.UnityConsole()
            .MinimumLevel.Debug()
            .CreateLogger();

        Log.Debug("114514");

        var ios = EasySerializerNative.AllocStringIoStream();
        var oarch = EasySerializerNative.AllocBinaryOutputArchive(ios);
        EasySerializerNative.WriteInt32ToOutputArchive(oarch, "int32", 134);
        EasySerializerNative.WriteStringToOutputArchive(oarch, "str", "66666");
        var cbuf = EasySerializerNative.GetIoStreamBuffer(ios);
        byte[] buf = new byte[cbuf.size];
        Marshal.Copy(cbuf.ptr, buf, 0, (int)cbuf.size);

        var iarch = EasySerializerNative.AllocBinaryInputArchive(ios);
        var val = EasySerializerNative.ReadInt32FromInputArchive(iarch, "int32");
        var str = EasySerializerNative.ReadStringFromInputArchive(iarch, "str");

        Debug.Log(val);
    }
	
	void Update()
	{
	}
}
