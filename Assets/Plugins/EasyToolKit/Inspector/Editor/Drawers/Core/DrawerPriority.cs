using System;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public enum DrawerPriorityLevel
    {
        ValuePriority,
        AttributePriority
    }

    public class DrawerPriority : IEquatable<DrawerPriority>, IComparable<DrawerPriority>
    {
        public static readonly DrawerPriority ValuePriority = new DrawerPriority(1);
        public static readonly DrawerPriority AttributePriority = new DrawerPriority(1000);

        public readonly double Value;

        public DrawerPriority(DrawerPriorityLevel level)
        {
            DrawerPriority priority;
            switch (level)
            {
                case DrawerPriorityLevel.ValuePriority:
                    priority = ValuePriority;
                    break;
                case DrawerPriorityLevel.AttributePriority:
                    priority = AttributePriority;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }

            Value = priority.Value;
        }

        public DrawerPriority(double value)
        {
            Value = value;
        }

        public static bool operator ==(DrawerPriority left, DrawerPriority right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;

            return Mathf.Approximately((float)left.Value, (float)right.Value);
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
