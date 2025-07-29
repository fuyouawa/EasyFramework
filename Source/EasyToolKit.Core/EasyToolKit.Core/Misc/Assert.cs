using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace EasyToolKit.Core
{
    public class AssertException : Exception
    {
        public AssertException(string message = null)
            : base(message.DefaultIfNullOrEmpty("Assertion failed"))
        {
        }
    }

    public static class Assert
    {
        private static void Condition([DoesNotReturnIf(false)] bool cond, string message)
        {
            if (!cond)
            {
                // if (Debugger.IsAttached)
                // {
                //     Debugger.Break();
                // }
                // else
                // {
                //     Environment.FailFast("Assert failed", new AssertException(new StackTrace()));
                // }
                throw new AssertException(message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsEqual<T>(T x, T y, string message)
        {
            Condition(EqualityComparer<T>.Default.Equals(y), message);
        }
        
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsEqual<T>(T x, T y)
        {
            IsEqual(x, y, $"Assert equal '{x}' and '{y}' failed.");
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotEqual<T>(T x, T y, string message)
        {
            Condition(!EqualityComparer<T>.Default.Equals(y), message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotEqual<T>(T x, T y)
        {
            IsNotEqual(x, y, $"Assert not equal '{x}' and '{y}' failed.");
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull(object target, string message)
        {
            Condition(target == null, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T target)
        {
            IsNull(target, $"Assert null '{target}' failed.");
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull(object target, string message)
        {
            Condition(target != null, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>(T target)
        {
            IsNotNull(target, $"Assert not null '{target}' failed.");
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue([DoesNotReturnIf(false)] bool cond, string message)
        {
            Condition(cond, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue([DoesNotReturnIf(false)] bool cond)
        {
            IsTrue(cond, $"Assert true '{cond}' failed.");
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue([DoesNotReturnIf(false)] bool? cond, string message)
        {
            Condition(cond == true, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue([DoesNotReturnIf(false)] bool? cond)
        {
            IsTrue(cond, $"Assert true '{cond}' failed.");
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse([DoesNotReturnIf(true)] bool cond, string message)
        {
            Condition(!cond, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse([DoesNotReturnIf(true)] bool cond)
        {
            IsFalse(cond, $"Assert false '{cond}' failed.");
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse([DoesNotReturnIf(true)] bool? cond, string message)
        {
            Condition(cond == false, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse([DoesNotReturnIf(true)] bool? cond)
        {
            IsFalse(cond, $"Assert false '{cond}' failed.");
        }
    }
}
