using System;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// 类型与编辑器的配对结构，用于存储需要被特定编辑器绘制的类型信息。
    /// </summary>
    public struct TypeEditorPair
    {
        /// <summary>
        /// 被绘制类型的完整名称。
        /// </summary>
        public string DrawnTypeName;
        
        /// <summary>
        /// 编辑器类型的完整名称。
        /// </summary>
        public string EditorTypeName;

        /// <summary>
        /// 创建一个只有被绘制类型的配对。
        /// </summary>
        /// <param name="drawnType">被绘制的类型</param>
        /// <exception cref="ArgumentNullException">当drawnType为null时抛出</exception>
        public TypeEditorPair(Type drawnType)
        {
            this.DrawnTypeName = drawnType != null
                ? TypeConverter.ToName(drawnType)
                : throw new ArgumentNullException(nameof(drawnType));
            this.EditorTypeName = string.Empty;
        }

        /// <summary>
        /// 创建一个包含被绘制类型和编辑器类型的配对。
        /// </summary>
        /// <param name="drawnType">被绘制的类型</param>
        /// <param name="editorType">编辑器类型</param>
        /// <exception cref="ArgumentNullException">当drawnType为null时抛出</exception>
        public TypeEditorPair(Type drawnType, Type editorType)
            : this(drawnType)
        {
            this.EditorTypeName = editorType != null
                ? TypeConverter.ToName(editorType)
                : string.Empty;
        }

        /// <summary>
        /// 比较两个TypeEditorPair是否相等。
        /// </summary>
        /// <param name="other">要比较的另一个TypeEditorPair</param>
        /// <returns>如果两个配对相等则返回true，否则返回false</returns>
        public bool Equals(TypeEditorPair other)
        {
            return other.EditorTypeName == this.EditorTypeName && other.DrawnTypeName == this.DrawnTypeName;
        }

        /// <summary>
        /// 获取当前实例的哈希代码。
        /// </summary>
        /// <returns>表示当前实例的哈希代码</returns>
        public override int GetHashCode()
        {
            return (this.EditorTypeName != null ? this.EditorTypeName.GetHashCode() * 7 : 0) ^
                   (this.DrawnTypeName != null ? this.DrawnTypeName.GetHashCode() * 13 : 0);
        }

        /// <summary>
        /// 确定指定的对象是否等于当前对象。
        /// </summary>
        /// <param name="obj">要与当前对象比较的对象</param>
        /// <returns>如果指定的对象等于当前对象，则为true，否则为false</returns>
        public override bool Equals(object obj)
        {
            return obj != null && obj is TypeEditorPair && this.Equals((TypeEditorPair)obj);
        }

        /// <summary>
        /// 判断两个TypeEditorPair是否相等。
        /// </summary>
        public static bool operator ==(TypeEditorPair x, TypeEditorPair y) => x.Equals(y);

        /// <summary>
        /// 判断两个TypeEditorPair是否不相等。
        /// </summary>
        public static bool operator !=(TypeEditorPair x, TypeEditorPair y) => !x.Equals(y);
    }
}
