using System;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public struct TypeDrawerPair
    {
        public string DrawnTypeName;
        public string EditorTypeName;

        public TypeDrawerPair(System.Type drawnType)
        {
            this.DrawnTypeName = !(drawnType == (System.Type)null)
                ? TypeConverter.ToName(drawnType)
                : throw new ArgumentNullException(nameof(drawnType));
            this.EditorTypeName = string.Empty;
        }

        public TypeDrawerPair(System.Type drawnType, System.Type editorType)
            : this(drawnType)
        {
            if (editorType == (System.Type)null)
                this.EditorTypeName = string.Empty;
            else
                this.EditorTypeName = TypeConverter.ToName(editorType);
        }

        public bool Equals(TypeDrawerPair other)
        {
            return other.EditorTypeName == this.EditorTypeName && other.DrawnTypeName == this.DrawnTypeName;
        }

        public override int GetHashCode()
        {
            return (this.EditorTypeName != null ? this.EditorTypeName.GetHashCode() * 7 : 0) ^
                   (this.DrawnTypeName != null ? this.DrawnTypeName.GetHashCode() * 13 : 0);
        }

        public override bool Equals(object obj)
        {
            return obj != null && !(obj.GetType() != typeof(TypeDrawerPair)) && this.Equals((TypeDrawerPair)obj);
        }

        public static bool operator ==(TypeDrawerPair x, TypeDrawerPair y) => x.Equals(y);

        public static bool operator !=(TypeDrawerPair x, TypeDrawerPair y) => !x.Equals(y);
    }
}
