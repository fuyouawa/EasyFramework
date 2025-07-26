using EasyToolKit.ThirdParty.OdinSerializer;
using System;

namespace EasyToolKit.Core
{

    public class DefaultCodeValueResolver : ICodeValueResolver
    {
        private readonly string _code;
        private readonly Type _resultType;
        private readonly Type _targetType;
        private bool _isChecked;
        private Func<object, object> _resolver;
        private string _error;

        public DefaultCodeValueResolver(string code, Type resultType, Type targetType)
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

        private bool HasErrorImpl(out string error, Type targetType = null)
        {
            if (_code.IsNullOrWhiteSpace())
            {
                error = "Code cannot be null or empty.";
                return true;
            }

            Type rootType = null;
            if (TryGetArgument("-t:", out var rootTypeText))
            {
                try
                {
                    rootType = TwoWaySerializationBinder.Default.BindToType(rootTypeText);
                }
                catch (Exception e)
                {
                    error = $"Get root type '{rootTypeText}' failed: {e.Message}";
                    return true;
                }
            }
            else
            {
                if (targetType == null)
                {
                    error = "Target type cannot be null when argument '-t:' is specified.";
                    return true;
                }
            }

            string path = null;
            if (!TryGetArgument("-p:", out path))
            {
                if (rootType != null)
                {
                    error = "Path argument '-p:' is required when '-t:' is specified.";
                    return true;
                }
                else
                {
                    path = _code;
                }
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

        private bool TryGetArgument(string argumentType, out string argumentContent)
        {
            var i = _code.IndexOf(argumentType, StringComparison.OrdinalIgnoreCase);
            if (i != -1)
            {
                i += argumentType.Length;
                var rest = _code[i..];
                argumentContent = rest.Split(' ')[0];
                return true;
            }

            argumentContent = null;
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
