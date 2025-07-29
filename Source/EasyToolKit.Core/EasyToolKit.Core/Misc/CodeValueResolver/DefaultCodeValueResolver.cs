using EasyToolKit.ThirdParty.OdinSerializer;
using JetBrains.Annotations;
using System;

namespace EasyToolKit.Core
{
    public class DefaultCodeValueResolver : ICodeValueResolver
    {
        private readonly string _code;
        private readonly Type _resultType;
        [CanBeNull] private readonly Type _targetType;
        private bool _isChecked;
        private Func<object, object> _resolver;
        private string _error;

        public DefaultCodeValueResolver(string code, Type resultType, [CanBeNull] Type targetType)
        {
            _code = code;
            _resultType = resultType;
            _targetType = targetType;
        }

        public bool HasError(out string error)
        {
            if (_isChecked)
            {
                error = _error;
                return error.IsNotNullOrEmpty();
            }

            var suc = HasErrorImpl(out error, _targetType);
            _isChecked = true;
            _error = error;
            return suc;
        }

        public static bool TryAnalyseCode(string code, [CanBeNull] Type targetType, out Type rootType, out string path, out string error)
        {
            rootType = null;
            path = null;
            error = null;

            if (TryGetArgument(code, "-t:", out var rootTypeText))
            {
                try
                {
                    rootType = TwoWaySerializationBinder.Default.BindToType(rootTypeText);
                }
                catch (Exception e)
                {
                    error = $"Get root type '{rootTypeText}' failed: {e.Message}";
                    return false;
                }
            }
            else
            {
                if (targetType == null)
                {
                    error = "Target type cannot be null when argument '-t:' is specified.";
                    return false;
                }
            }

            path = null;
            if (!TryGetArgument(code, "-p:", out path))
            {
                if (rootType != null)
                {
                    error = "Path argument '-p:' is required when '-t:' is specified.";
                    return false;
                }
                else
                {
                    path = code;
                }
            }

            return true;
        }

        private static bool TryGetArgument(string code, string argumentType, out string argumentContent)
        {
            var i = code.IndexOf(argumentType, StringComparison.OrdinalIgnoreCase);
            if (i != -1)
            {
                i += argumentType.Length;
                var rest = code[i..];
                argumentContent = rest.Split(' ')[0];
                return true;
            }

            argumentContent = null;
            return false;
        }

        private bool HasErrorImpl(out string error, Type targetType = null)
        {
            if (_code.IsNullOrWhiteSpace())
            {
                error = "Code cannot be null or empty.";
                return true;
            }

            if (!TryAnalyseCode(_code, targetType, out var rootType, out var path, out error))
            {
                return true;
            }

            try
            {
                if (rootType != null)
                {
                    var getter = ReflectionUtility.CreateStaticValueGetter(_resultType, rootType, path);
                    _resolver = o => getter();
                }
                else
                {
                    var getter = ReflectionUtility.CreateInstanceValueGetter(targetType, _resultType, path);
                    _resolver = o => getter(o);
                }
            }
            catch (Exception e)
            {
                error = $"Create value getter failed: {e.Message}";
                return true;
            }

            error = null;
            return false;
        }

        public object ResolveWeak(object context)
        {
            return _resolver(context);
        }
    }

    public class DefaultCodeValueResolver<T> : DefaultCodeValueResolver, ICodeValueResolver<T>
    {
        public DefaultCodeValueResolver(string code, Type targetType) : base(code, typeof(T), targetType)
        {
        }

        public T Resolve(object context)
        {
            return (T)ResolveWeak(context);
        }
    }
}
