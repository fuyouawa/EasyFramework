using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Debug = UnityEngine.Debug;

namespace EasyToolKit.Core
{
    public class AssertException : Exception
    {
        public AssertException()
            : this("Assertion failed")
        {
        }

        public AssertException(string message)
            : base(message.DefaultIfNullOrEmpty("Assertion failed"))
        {
        }
    }

    public static class Assert
    {
        private static void Condition([DoesNotReturnIf(false)] bool cond)
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
                throw new AssertException();
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Equal<T>(T x, T y)
        {
            Condition(EqualityComparer<T>.Default.Equals(y));
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void NotEqual<T>(T x, T y)
        {
            Condition(!EqualityComparer<T>.Default.Equals(y));
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Null(object target)
        {
            Condition(target == null);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void NotNull(object target)
        {
            Condition(target != null);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void True([DoesNotReturnIf(false)] bool cond)
        {
            Condition(cond);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void True([DoesNotReturnIf(false)] bool? cond)
        {
            Condition(cond == true);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void False([DoesNotReturnIf(true)] bool cond)
        {
            Condition(!cond);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void False([DoesNotReturnIf(true)] bool? cond)
        {
            Condition(cond == false);
        }
    }
}
