using System;

namespace EasyToolKit.Core
{
    public static class CodeValueResolver
    {
        public static ICodeValueResolver CreateWeak(string code, Type resultType = null, Type targetType = null, bool needStartFlag = false)
        {
            if (needStartFlag)
            {
                if (code.StartsWith("@"))
                {
                    return new DefaultCodeValueResolver(code[1..], resultType, targetType);
                }
                else
                {
                    return new PrimitiveCodeValueResolver(code);
                }
            }
            else
            {
                return new DefaultCodeValueResolver(code, resultType, targetType);
            }
        }

        public static ICodeValueResolver<T> Create<T>(string code, Type targetType = null, bool needStartFlag = false)
        {
            if (needStartFlag)
            {
                if (code.StartsWith("@"))
                {
                    return new DefaultCodeValueResolver<T>(code[1..], targetType);
                }
                else
                {
                    if (typeof(T) != typeof(string))
                    {
                        throw new ArgumentException("When using primitive code, the result type must be string.");
                    }
                    return new PrimitiveCodeValueResolver<T>(code);
                }
            }
            else
            {
                return new DefaultCodeValueResolver<T>(code, targetType);
            }
        }
    }
}
