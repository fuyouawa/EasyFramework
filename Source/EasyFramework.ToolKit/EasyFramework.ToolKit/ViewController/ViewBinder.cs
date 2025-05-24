using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EasyFramework.Core;
using EasyFramework.Serialization;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Conditional("UNITY_EDITOR")]
    public class AutoBindingAttribute : PropertyAttribute
    {
        public AutoBindingAttribute()
        {
        }
    }

    public enum ViewBindAccess
    {
        Public,
        Protected,
        Private
    }

    public class ViewBinder : SerializedMonoBehaviour
    {
        [SerializeField] private bool _noOwningController;
        [SerializeField] private Component _owningController;
        [SerializeField] private bool _bindGameObject;
        [NonSerialized, OdinSerialize] private Type _bindComponentType;
        [NonSerialized, OdinSerialize] private Type _specificBindType;

        [SerializeField] private ViewBindAccess _bindAccess;

        [SerializeField] private string _bindName;
        [SerializeField] private bool _autoBindName = true;
        [SerializeField] private bool _processBindName = true;

        [SerializeField] private bool _useDocumentComment = true;

        [ShowIf(nameof(ShowAutoAddParaToComment))]
        [SerializeField] private bool _autoAddParaToComment = true;

        [TextArea(4, 10)]
        [SerializeField] private string _comment;

        [SerializeField] private bool _isInitialized;

        public ViewBindAccess BindAccess => _bindAccess;
        public IViewController OwningController => _noOwningController ? null : (IViewController)_owningController;

        private bool ShowAutoAddParaToComment => _useDocumentComment;

        private List<string> GetCommentSplits()
        {
            if (_comment.IsNullOrWhiteSpace())
            {
                return null;
            }

            var comment = _comment.Replace("\r\n", "\n");
            var commentSplits = comment.Split('\n').ToList();

            if (_autoAddParaToComment)
            {
                for (int i = 0; i < commentSplits.Count; i++)
                {
                    commentSplits[i] = "<para>" + commentSplits[i] + "</para>";
                }
            }

            commentSplits.Insert(0, "<summary>");
            commentSplits.Add("</summary>");

            return commentSplits;
        }

        public string GetComment()
        {
            var splits = GetCommentSplits();
            if (splits.IsNullOrEmpty())
                return string.Empty;
            return string.Join("\n", splits);
        }

        public UnityEngine.Object GetBindObject()
        {
            if (_bindGameObject)
            {
                return gameObject;
            }

            if (_bindComponentType == null)
            {
                throw new NullReferenceException("The field '_bindComponentType' is null!");
            }

            return GetComponent(_bindComponentType);
        }

        public Type GetBindType()
        {
            if (_bindGameObject)
            {
                return typeof(GameObject);
            }

            return _specificBindType;
        }

        private static string ProcessName(string name, ViewBindAccess access)
        {
            if (name.IsNullOrWhiteSpace())
                return name;

            //TODO 更多情况的处理
            if (access == ViewBindAccess.Public)
            {
                return char.ToUpper(name[0]) + name[1..];
            }

            name = char.ToLower(name[0]) + name[1..];
            return '_' + name;
        }

        public string GetBindName()
        {
            var bindName = _bindName;

            if (_autoBindName)
            {
                bindName = gameObject.name;
            }

            if (_processBindName)
            {
                return ProcessName(bindName, _bindAccess);
            }

            return bindName;
        }
    }
}
