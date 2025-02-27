using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Serializable]
    public class OtherViewBinderConfig
    {
        public GameObject Target;
        [ShowIf(nameof(ShowConfig))]
        public ViewBinderEditorConfig EditorConfig;

        private bool ShowConfig => Target != null;
    }
    
    [Serializable]
    public class OtherViewBinderConfigs
    {
        public List<OtherViewBinderConfig> Collection;
    }

    public sealed class OtherViewBinders : MonoBehaviour
    {
        public OtherViewBinderConfigs Configs;
    }
}
