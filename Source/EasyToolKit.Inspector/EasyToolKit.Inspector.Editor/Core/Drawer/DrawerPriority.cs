using System;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class DrawerPriority : IEquatable<DrawerPriority>, IComparable<DrawerPriority>
    {
        public static readonly DrawerPriority DefaultPriority = new DrawerPriority(DrawerPriorityLevel.Default);
        public static readonly DrawerPriority ValuePriority = new DrawerPriority(DrawerPriorityLevel.Value);
        public static readonly DrawerPriority AttributePriority = new DrawerPriority(DrawerPriorityLevel.Attribute);
        public static readonly DrawerPriority SuperPriority = new DrawerPriority(DrawerPriorityLevel.Super);

        public readonly double Value;

        public DrawerPriority(double value)
        {
            Value = value;
        }

        public static bool operator ==(DrawerPriority left, DrawerPriority right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;

            return left.Value.IsApproximatelyOf(right.Value);
        }

        public static bool operator !=(DrawerPriority left, DrawerPriority right)
        {
            return !(left == right);
        }

        public static bool operator >(DrawerPriority left, DrawerPriority right)
        {
            if (left == right) return false;
            if (left.Value > right.Value) return true;
            return false;
        }

        public static bool operator <(DrawerPriority left, DrawerPriority right)
        {
            if (left == right) return false;
            if (left.Value < right.Value) return true;
            return false;
        }


        public static bool operator >=(DrawerPriority left, DrawerPriority right)
        {
            if (left.Value.IsApproximatelyOf(right.Value) || left.Value > right.Value) return true;
            return false;
        }

        public static bool operator <=(DrawerPriority left, DrawerPriority right)
        {
            if (left.Value.IsApproximatelyOf(right.Value) || left.Value < right.Value) return true;
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is DrawerPriority priority)
            {
                return this == priority;
            }
            return false;
        }

        public bool Equals(DrawerPriority other)
        {
            return this == other;
        }

        public int CompareTo(DrawerPriority other)
        {
            if (this > other)
            {
                return 1;
            }

            if (this < other)
            {
                return -1;
            }

            return 0;
        }
    }
}
