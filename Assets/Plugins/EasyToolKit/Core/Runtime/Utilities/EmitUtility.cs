using System;
using System.Reflection;

namespace EasyToolKit.Core
{
    public static class EmitUtility
    {
        /// <summary>
        /// Creates a fast delegate method which calls a given parameterless weakly typed instance method.
        /// </summary>
        /// <param name="methodInfo">The method info instance which is used.</param>
        /// <returns>A delegate which calls the method and returns the result, except it's hundreds of times faster than MethodInfo.Invoke.</returns>
        public static Action<object, TArg1> CreateWeakInstanceMethodCaller<TArg1>(MethodInfo methodInfo)
        {
            return Internal.OdinSerializer.Utilities.EmitUtilities.CreateWeakInstanceMethodCaller<TArg1>(methodInfo);
        }
    }
}
