using System;

namespace EasyToolKit.Core
{
    public class PrimitiveCodeValueResolver : ICodeValueResolver
    {
        private readonly string _primitiveCode;

        public PrimitiveCodeValueResolver(string primitiveCode)
        {
            _primitiveCode = primitiveCode;
        }

        public bool HasError(out string error)
        {
            error = null;
            return false;
        }

        public object ResolveWeak(object context)
        {
            return _primitiveCode;
        }
    }

    public class PrimitiveCodeValueResolver<T> : PrimitiveCodeValueResolver, ICodeValueResolver<T>
    {
        public PrimitiveCodeValueResolver(string primitiveCode) : base(primitiveCode)
        {
        }

        public T Resolve(object context)
        {
            return (T)ResolveWeak(context);
        }
    }
}
