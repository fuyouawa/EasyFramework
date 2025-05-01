using System;

namespace EasyFramework.Core
{
    public class NativeException : Exception
    {
        private readonly NativeErrorCode _errorCode;

        public NativeException(NativeErrorCode errorCode, string message)
            : base(message)
        {
            _errorCode = errorCode;
        }

        public NativeErrorCode ErrorCode => _errorCode;
    }
}
