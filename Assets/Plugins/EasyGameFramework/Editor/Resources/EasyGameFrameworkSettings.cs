using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    [Serializable]
    public class EasyControlSettings
    {
        [Serializable]
        public class ViewModelDefaultSettings
        {
            [LabelText("生成路径")]
            [FolderPath(ParentFolder = "Assets")]
            public string GenerateDir = "Scripts/Ui";
            [LabelText("命名空间")]
            public string Namespace;
        }
        
        [Serializable]
        public class BounderDefaultSettings
        {
            [LabelText("自动命名空间")]
            public bool AutoNamingNotations = true;
            [LabelText("注释")]
            public string Comment;
            [LabelText("访问标识符")]
            public EasyControlBindAccess Access;
            [LabelText("注释自动添加段落xml")]
            public bool AutoAddCommentPara = true;
        }

        [HideLabel, BoxGroup("视图模型-默认值设置")]
        public ViewModelDefaultSettings ViewModelDefault = new();
        [HideLabel, BoxGroup("被绑定者-默认值设置")]
        public BounderDefaultSettings BounderDefault = new();
    }
    
    [EditorResourcesAssetPath]
    public class EasyGameFrameworkSettings : ScriptableObjectSingleton<EasyGameFrameworkSettings>
    {
        [LabelText("EasyControl设置")]
        public EasyControlSettings EasyControlSettings = new();
    }
}
