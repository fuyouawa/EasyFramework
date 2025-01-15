using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using EasyFramework;
using JetBrains.Annotations;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyGameFramework
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class FontAssetPreset
    {
        public TMP_FontAsset FontAsset;
        public Material Material;
    }

    [Serializable]
    public class TextPropertiesPreset
    {
        public float FontSize;
        public Color FontColor;
    }

    public class FontAssetPresetDictionary : Dictionary<string, FontAssetPreset>
    {
    }

    public class TextPropertiesPresetDictionary : Dictionary<string, TextPropertiesPreset>
    {
    }

    [ScriptableObjectSingletonAssetPath("Assets/Resources/Fonts")]
    [ShowOdinSerializedPropertiesInInspector]
    public class UiTextPresetsManager : ScriptableObjectSingleton<UiTextPresetsManager>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private SerializationData _serializationData;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref this._serializationData);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref this._serializationData);
        }

        public FontAssetPresetDictionary FontAssetPresets = new();
        public TextPropertiesPresetDictionary TextPropertiesPresets = new();
        public string DefaultFontAssetPresetId;
        public string DefaultTextPropertiesPresetId;
    }
}
