using System;

namespace EasyFramework.Core
{
    public static class DefaultTypeMatchRules
    {
        public static Type ExactMatch(TypeMatchIndex matchIndex, Type[] targets, ref bool stopMatch)
        {
            if (matchIndex.Type.IsGenericTypeDefinition) return null;
            if (targets.Length != matchIndex.Targets.Length) return null;

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != matchIndex.Targets[i]) return null;
            }

            return matchIndex.Type;
        }

        public static Type GenericSingleTargetMatch(TypeMatchIndex matchIndex, Type[] targets, ref bool stopMatch)
        {
            if (!matchIndex.Type.IsGenericTypeDefinition) return null;
            if (targets.Length != 1) return null;
            if (!matchIndex.Targets[0].IsGenericType || !targets[0].IsGenericType) return null;
            if (matchIndex.Targets[0].GetGenericTypeDefinition() != targets[0].GetGenericTypeDefinition()) return null;

            var matchArgs = matchIndex.Type.GetGenericArguments();
            var matchTargetArgs = matchIndex.Targets[0].GetGenericArguments();
            var targetArgs = targets[0].GetGenericArguments();

            if (matchArgs.Length != matchTargetArgs.Length || matchArgs.Length != targetArgs.Length) return null;

            if (!matchIndex.Type.AreGenericConstraintsSatisfiedBy(targetArgs)) return null;

            return matchIndex.Type.MakeGenericType(targetArgs);
        }


        public static Type TargetsSatisfyGenericParameterConstraints(TypeMatchIndex matchIndex, Type[] targets,
            ref bool stopMatch)
        {
            for (int i = 0; i < matchIndex.Targets.Length; i++)
            {
                if (!matchIndex.Targets[i].IsGenericParameter) return null;
            }

            if (matchIndex.Type.IsGenericType && matchIndex.Type.AreGenericConstraintsSatisfiedBy(targets))
            {
                return matchIndex.Type.MakeGenericType(targets);
            }

            return null;
        }

        public static Type GenericParameterInference(TypeMatchIndex matchIndex, Type[] targets,
            ref bool stopMatch)
        {
            Type[] inferTargets;

            // Make sure we can apply generic parameter inference to the match info
            {
                if (!matchIndex.Type.IsGenericType) return null;

                int genericParameterTargetCount = 0;

                for (int i = 0; i < matchIndex.Targets.Length; i++)
                {
                    if (matchIndex.Targets[i].IsGenericParameter)
                    {
                        genericParameterTargetCount++;
                    }
                    else if (matchIndex.Targets[i] != targets[i])
                    {
                        // Everything but generic parameters must match exactly
                        return null;
                    }
                }

                if (genericParameterTargetCount == 0) return null;

                if (genericParameterTargetCount != targets.Length)
                {
                    inferTargets = new Type[genericParameterTargetCount];
                    int count = 0;
                    for (int i = 0; i < matchIndex.Targets.Length; i++)
                    {
                        if (matchIndex.Targets[i].IsGenericParameter)
                        {
                            inferTargets[count++] = targets[i];
                        }
                    }
                }
                else
                {
                    inferTargets = targets;
                }
            }

            Type[] inferredArgs;

            if (matchIndex.Type.TryInferGenericArguments(out inferredArgs, inferTargets))
            {
                return matchIndex.Type.GetGenericTypeDefinition().MakeGenericType(inferredArgs);
            }

            return null;
        }

        public static Type NestedInSameGenericType(TypeMatchIndex matchIndex, Type[] targets,
            ref bool stopMatch)
        {
            if (targets.Length != 1) return null;

            var target = targets[0];

            if (!matchIndex.Type.IsNested || !matchIndex.Targets[0].IsNested || !target.IsNested) return null;

            if (!matchIndex.Type.DeclaringType.IsGenericType ||
                !matchIndex.Targets[0].DeclaringType.IsGenericType ||
                !target.DeclaringType.IsGenericType)
            {
                return null;
            }

            if (matchIndex.Type.DeclaringType.GetGenericTypeDefinition() != matchIndex.Targets[0].DeclaringType.GetGenericTypeDefinition() ||
                matchIndex.Type.DeclaringType.GetGenericTypeDefinition() != target.DeclaringType.GetGenericTypeDefinition()) return null;

            var args = target.GetGenericArguments();

            if (matchIndex.Type.AreGenericConstraintsSatisfiedBy(args))
            {
                return matchIndex.Type.MakeGenericType(args);
            }

            return null;
        }
    }
}
