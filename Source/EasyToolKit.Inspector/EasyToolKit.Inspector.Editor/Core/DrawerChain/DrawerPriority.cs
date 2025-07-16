using System;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public enum DrawerPriorityLevel
    {
        Value,
        Attribute,
        Super
    }

    public class DrawerPriority : IEquatable<DrawerPriority>, IComparable<DrawerPriority>
    {
        public static readonly DrawerPriority DefaultPriority = new DrawerPriority(0);
        public static readonly DrawerPriority ValuePriority = new DrawerPriority(1);
        public static readonly DrawerPriority AttributePriority = new DrawerPriority(1000);
        public static readonly DrawerPriority SuperPriority = new DrawerPriority(100000);

        public readonly int Value;

        public DrawerPriority(DrawerPriorityLevel level)
        {
            var priority = level switch
            {
                DrawerPriorityLevel.Value => ValuePriority,
                DrawerPriorityLevel.Attribute => AttributePriority,
                DrawerPriorityLevel.Super => SuperPriority,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };

            Value = priority.Value;
        }

        public DrawerPriority(int value)
        {
            Value = value;
        }

        public static bool operator ==(DrawerPriority left, DrawerPriority right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;

            return left.Value == right.Value;
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
            if (left.Value >= right.Value) return true;
            return false;
        }

        public static bool operator <=(DrawerPriority left, DrawerPriority right)
        {
            if (left.Value <= right.Value) return true;
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
