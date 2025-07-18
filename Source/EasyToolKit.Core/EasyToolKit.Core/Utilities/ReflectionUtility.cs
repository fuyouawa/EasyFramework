using EasyToolKit.ThirdParty.OdinSerializer.Utilities;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Collections;
using System.Linq;

namespace EasyToolKit.Core
{
    //TODO 版权问题
    public class ReflectionUtility
    {
        private enum PathStepType
        {
            Member,
            WeakListElement,
            StrongListElement,
            ArrayElement
        }

        private struct PathStep
        {
            public readonly PathStepType StepType;
            public readonly MemberInfo Member;
            public readonly int ElementIndex;
            public readonly Type ElementType;
            public readonly MethodInfo StrongListGetItemMethod;

            public PathStep(MemberInfo member)
            {
                this.StepType = PathStepType.Member;
                this.Member = member;
                this.ElementIndex = -1;
                this.ElementType = null;
                this.StrongListGetItemMethod = null;
            }

            public PathStep(int elementIndex)
            {
                this.StepType = PathStepType.WeakListElement;
                this.Member = null;
                this.ElementIndex = elementIndex;
                this.ElementType = null;
                this.StrongListGetItemMethod = null;
            }

            public PathStep(int elementIndex, Type strongListElementType, bool isArray)
            {
                this.StepType = isArray ? PathStepType.ArrayElement : PathStepType.StrongListElement;
                this.Member = null;
                this.ElementIndex = elementIndex;
                this.ElementType = strongListElementType;
                this.StrongListGetItemMethod =
                    typeof(IList<>).MakeGenericType(strongListElementType).GetMethod("get_Item");
            }
        }

        public static Func<TResult> CreateValueGetter<TResult>(Type rootType, string path, bool allowEmit = true)
        {
            if (rootType == null)
            {
                throw new ArgumentNullException("rootType");
            }

            var resultType = typeof(TResult);
            bool rootIsStatic;
            var memberPath = GetMemberPath(rootType, ref resultType, path, out rootIsStatic, isSet: false);

            if (rootIsStatic == false)
            {
                throw new ArgumentException(
                    "Given path root is not static; use the generic overload with a target type.");
            }

            var slowDelegate = CreateSlowDeepStaticValueGetterDelegate(memberPath);
            return () => (TResult)slowDelegate();
        }

        public static Func<TTarget, TResult> CreateValueGetter<TTarget, TResult>(string path, bool allowEmit = true)
        {
            var resultType = typeof(TResult);
            bool rootIsStatic;
            var memberPath = GetMemberPath(typeof(TTarget), ref resultType, path, out rootIsStatic, isSet: false);

            if (rootIsStatic)
            {
                throw new ArgumentException(
                    "Given path root is static; use the generic overload without a target type.");
            }

            var slowDelegate = CreateSlowDeepInstanceValueGetterDelegate(memberPath);
            return (target) => (TResult)slowDelegate(target);
        }

        private static Func<object> CreateSlowDeepStaticValueGetterDelegate(List<PathStep> memberPath)
        {
            return () =>
            {
                object currentInstance = null;

                for (int i = 0; i < memberPath.Count; i++)
                {
                    currentInstance = SlowGetMemberValue(memberPath[i], currentInstance);
                }

                return currentInstance;
            };
        }

        private static Func<object, object> CreateSlowDeepInstanceValueGetterDelegate(List<PathStep> memberPath)
        {
            return (object instance) =>
            {
                object currentInstance = instance;

                for (int i = 0; i < memberPath.Count; i++)
                {
                    currentInstance = SlowGetMemberValue(memberPath[i], currentInstance);
                }

                return currentInstance;
            };
        }

        private static object SlowGetMemberValue(PathStep step, object instance)
        {
            switch (step.StepType)
            {
                case PathStepType.Member:
                {
                    FieldInfo field = step.Member as FieldInfo;
                    if (field != null)
                    {
                        return field.GetValue(instance);
                    }

                    PropertyInfo prop = step.Member as PropertyInfo;
                    if (prop != null)
                    {
                        return prop.GetValue(instance, null);
                    }

                    MethodInfo method = step.Member as MethodInfo;
                    if (method != null)
                    {
                        return method.Invoke(instance, null);
                    }

                    throw new NotSupportedException(step.Member.GetType().GetNiceName());
                }

                case PathStepType.WeakListElement:
                    return ((IList)instance)[step.ElementIndex];

                case PathStepType.ArrayElement:
                    return ((Array)instance).GetValue(step.ElementIndex);

                case PathStepType.StrongListElement:
                    return step.StrongListGetItemMethod.Invoke(instance, new object[] { step.ElementIndex });

                default:
                    throw new NotImplementedException(step.StepType.ToString());
            }
        }

        private static List<PathStep> GetMemberPath(Type rootType, ref Type resultType, string path,
            out bool rootIsStatic, bool isSet)
        {
            if (path.IsNullOrWhitespace())
            {
                throw new ArgumentException("Invalid path; is null or whitespace.");
            }

            rootIsStatic = false;
            List<PathStep> result = new List<PathStep>();
            string[] steps = path.Split('.');

            Type currentType = rootType;

            for (int i = 0; i < steps.Length; i++)
            {
                string step = steps[i];

                bool expectMethod = false;

                if (step.StartsWith("[", StringComparison.InvariantCulture) &&
                    step.EndsWith("]", StringComparison.InvariantCulture))
                {
                    int index;
                    string indexStr = step.Substring(1, step.Length - 2);

                    if (!int.TryParse(indexStr, out index))
                    {
                        throw new ArgumentException("Couldn't parse an index from the path step '" + step + "'.");
                    }

                    // We need to check the current type to see if we can treat it as a list

                    if (currentType.IsArray)
                    {
                        Type elementType = currentType.GetElementType();
                        result.Add(new PathStep(index, elementType, true));
                        currentType = elementType;
                    }
                    else if (currentType.ImplementsOpenGenericInterface(typeof(IList<>)))
                    {
                        Type elementType =
                            currentType.GetArgumentsOfInheritedOpenGenericInterface(typeof(IList<>))[0];
                        result.Add(new PathStep(index, elementType, false));
                        currentType = elementType;
                    }
                    else if (typeof(IList).IsAssignableFrom(currentType))
                    {
                        result.Add(new PathStep(index));
                        currentType = typeof(object);
                    }
                    else
                    {
                        throw new ArgumentException("Cannot get elements by index from the type '" +
                                                    currentType.Name +
                                                    "'.");
                    }

                    continue;
                }

                if (step.EndsWith("()", StringComparison.InvariantCulture))
                {
                    expectMethod = true;
                    step = step.Substring(0, step.Length - 2);
                }

                var member = GetStepMember(currentType, step, expectMethod);

                if (member.IsStatic())
                {
                    if (currentType == rootType)
                    {
                        rootIsStatic = true;
                    }
                    else
                    {
                        throw new ArgumentException("The non-root member '" + step +
                                                    "' is static; use that member as the path root instead.");
                    }
                }

                currentType = member.GetReturnType();

                if (expectMethod && (currentType == null || currentType == typeof(void)))
                {
                    throw new ArgumentException("The method '" + member.Name +
                                                "' has no return type and cannot be part of a deep reflection path.");
                }

                result.Add(new PathStep(member));
            }

            if (resultType == null)
            {
                resultType = currentType;
            }
            // Objects can always be assigned to everything else
            else if (currentType != typeof(object) && resultType.IsAssignableFrom(currentType) == false)
            {
                throw new ArgumentException("Last member '" + result[result.Count - 1].Member.Name + "' of path '" +
                                            path + "' contains type '" + currentType.AssemblyQualifiedName +
                                            "', which is not assignable to expected type '" +
                                            resultType.AssemblyQualifiedName + "'.");
            }

            return result;
        }

        private static MemberInfo GetStepMember(Type owningType, string name, bool expectMethod)
        {
            MemberInfo result = null;
            MemberInfo[] possibleMembers = owningType.GetAllMembers(name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static |
                BindingFlags.FlattenHierarchy).ToArray();
            int stepMethodParameterCount = int.MaxValue;

            for (int j = 0; j < possibleMembers.Length; j++)
            {
                MemberInfo member = possibleMembers[j];

                if (expectMethod)
                {
                    MethodInfo method = member as MethodInfo;

                    if (method != null)
                    {
                        int parameterCount = method.GetParameters().Length;

                        if (result == null || parameterCount < stepMethodParameterCount)
                        {
                            result = method;
                            stepMethodParameterCount = parameterCount;
                        }
                    }
                }
                else
                {
                    if (member is MethodInfo)
                    {
                        throw new ArgumentException("Found method member for name '" + name +
                                                    "', but expected a field or property.");
                    }

                    result = member;
                    break;
                }
            }

            if (result == null)
            {
                throw new ArgumentException("Could not find expected " +
                                            (expectMethod ? "method" : "field or property") + " '" + name +
                                            "' on type '" + owningType.GetNiceName() +
                                            "' while parsing reflection path.");
            }

            if (expectMethod && stepMethodParameterCount > 0)
            {
                throw new NotSupportedException("Method '" + result.GetNiceName() + "' has " +
                                                stepMethodParameterCount +
                                                " parameters, but method parameters are currently not supported.");
            }

            if ((result is FieldInfo || result is PropertyInfo || result is MethodInfo) == false)
            {
                throw new NotSupportedException("Members of type " + result.GetType().GetNiceName() +
                                                " are not support; only fields, properties and methods are supported.");
            }

            return result;
        }
    }
}
