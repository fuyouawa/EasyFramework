using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyFramework.Core
{
    public class TypeMatchIndex
    {
        public Type Type { get; }
        public int Priority { get; }
        public Type[] Targets { get; }

        public TypeMatchIndex(Type type, int priority, Type[] targets)
        {
            Type = type;
            Priority = priority;
            Targets = targets;
        }
    }

    public class TypeMatchResult
    {
        public TypeMatchIndex MatchIndex { get; }
        public Type MatchedType { get; }
        public Type[] MatchTargets { get; }
        public TypeMatchRule MatchRule { get; }

        public TypeMatchResult(TypeMatchIndex matchIndex, Type matchedType, Type[] matchTargets,
            TypeMatchRule matchRule)
        {
            MatchIndex = matchIndex;
            MatchedType = matchedType;
            MatchTargets = matchTargets;
            MatchRule = matchRule;
        }
    }

    public delegate Type TypeMatchRule(TypeMatchIndex matchIndex, Type[] targets, ref bool stopMatch);

    public class TypeMatcher
    {
        private TypeMatchIndex[] _matchIndices;
        private readonly List<TypeMatchRule> _matchRules = new List<TypeMatchRule>();

        public void SetTypeMatchIndexs(IEnumerable<TypeMatchIndex> matchIndices)
        {
            _matchIndices = matchIndices.ToArray();
        }

        public void AddMatchRule(TypeMatchRule rule)
        {
            _matchRules.Add(rule);
        }

        public void RemoveMatchRule(TypeMatchRule rule)
        {
            _matchRules.Remove(rule);
        }

        public TypeMatchResult[] Match(Type[] targets)
        {
            var results = new List<TypeMatchResult>();

            foreach (var index in _matchIndices)
            {
                if (index.Targets.Length != targets.Length)
                    continue;

                foreach (var rule in _matchRules)
                {
                    bool stop = false;
                    var match = rule(index, targets, ref stop);
                    if (match != null)
                        results.Add(new TypeMatchResult(index, match, targets, rule));

                    if (stop)
                        break;
                }
            }

            return results
                .OrderByDescending(result => result.MatchIndex.Priority)
                .ToArray();
        }
    }
}
