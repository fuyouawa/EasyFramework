using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Core
{
    public class TypeMatchIndex
    {
        public Type Type { get; set; }
        public int Priority { get; set; }
        public Type[] Targets { get; set; }

        public TypeMatchIndex(Type type, int priority, Type[] targets)
        {
            Type = type;
            Priority = priority;
            Targets = targets ?? Type.EmptyTypes;
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
        private static readonly Dictionary<int, TypeMatchResult[]> MergedResultsCache = new Dictionary<int, TypeMatchResult[]>();

        private List<TypeMatchIndex> _matchIndices;
        private readonly List<TypeMatchRule> _matchRules = new List<TypeMatchRule>();

        private readonly Dictionary<int, TypeMatchResult[]> _matchResultsCache = new Dictionary<int, TypeMatchResult[]>();

        public TypeMatcher(bool addDefaultMatchRules = true)
        {
            if (addDefaultMatchRules)
            {
                AddDefaultMatchRules();
            }
        }

        public void AddTypeMatchIndices(IEnumerable<TypeMatchIndex> matchIndices)
        {
            _matchIndices.AddRange(matchIndices);
            ClearCache();
        }

        public void SetTypeMatchIndices(IEnumerable<TypeMatchIndex> matchIndices)
        {
            _matchIndices = matchIndices.ToList();
            ClearCache();
        }

        public void AddMatchRule(TypeMatchRule rule)
        {
            _matchRules.Add(rule);
            ClearCache();
        }

        public void RemoveMatchRule(TypeMatchRule rule)
        {
            _matchRules.Remove(rule);
            ClearCache();
        }

        private void ClearCache()
        {
            _matchResultsCache.Clear();
            MergedResultsCache.Clear();
        }

        public TypeMatchResult[] GetCachedMatches(params Type[] targets)
        {
            var hash = new HashCode();
            foreach (var target in targets)
            {
                hash.Add(target);
            }
            var hashCode = hash.ToHashCode();

            if (_matchResultsCache.TryGetValue(hashCode, out var ret))
            {
                return ret;
            }

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

            ret = results
                .OrderByDescending(result => result.MatchIndex.Priority)
                .ToArray();
            _matchResultsCache[hashCode] = ret;
            return ret;
        }

        public static TypeMatchResult[] GetCachedMergedResults(IReadOnlyList<TypeMatchResult[]> resultsList)
        {
            if (resultsList.Count == 0)
            {
                return Array.Empty<TypeMatchResult>();
            }

            if (resultsList.Count == 1)
            {
                return resultsList[0];
            }

            var hash = new HashCode();
            foreach (var value in resultsList)
            {
                hash.Add(value);
            }
            var hashCode = hash.ToHashCode();

            if (MergedResultsCache.TryGetValue(hashCode, out var results))
            {
                return results;
            }

            results = resultsList
                .SelectMany(x => x)
                .OrderByDescending(result => result.MatchIndex.Priority)
                .ToArray();
            MergedResultsCache[hashCode] = results;
            return results;
        }

        private void AddDefaultMatchRules()
        {
            _matchRules.Add(DefaultTypeMatchRules.ExactMatch);
            _matchRules.Add(DefaultTypeMatchRules.GenericSingleTargetMatch);
            _matchRules.Add(DefaultTypeMatchRules.TargetsSatisfyGenericParameterConstraints);
            _matchRules.Add(DefaultTypeMatchRules.GenericParameterInference);
            _matchRules.Add(DefaultTypeMatchRules.NestedInSameGenericType);
        }
    }
}
