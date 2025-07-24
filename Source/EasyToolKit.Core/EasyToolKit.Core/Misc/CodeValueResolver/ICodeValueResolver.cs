using System;
using EasyToolKit.ThirdParty.OdinSerializer;

namespace EasyToolKit.Core
{
    public interface ICodeValueResolver
    {
        bool HasError(out string error);
        object WeakResolve(object context);
    }

    public interface ICodeValueResolver<T> : ICodeValueResolver
    {
        T Resolve(object context);
    }
}
