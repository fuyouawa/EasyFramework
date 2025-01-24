using System;
using System.Collections.Generic;
using EasyFramework;
using UnityEngine;



public partial class JBJB : MonoBehaviour
{
    public SerializedVariant Variant = new SerializedVariant(123);
    public SerializedAny Any;
    public MethodPicker Picker = new MethodPicker();
}
