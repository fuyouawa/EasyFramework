using EasyFramework.Serialization;
using System.Reflection;
using System.Text;
using System;

namespace EasyFramework.ToolKit
{
    public class GameConsoleCommand
    {
        public MethodInfo Call { get; }
        public MethodInfo ExampleGetter { get; }
        public GameConsoleCommandAttribute Attribute { get; }
        public GameConsoleCommandExampleAttribute ExampleAttribute { get; }

        public ParameterInfo Parameter { get; }

        private static MethodInfo _methodOfInternalSendWithArg;

        static GameConsoleCommand()
        {
            _methodOfInternalSendWithArg =
                typeof(GameConsoleCommand).GetMethod(nameof(InternalSendWithArg),
                    BindingFlagsHelper.NonPublicInstance());
        }

        public GameConsoleCommand(MethodInfo call, MethodInfo exampleGetter)
        {
            Call = call;
            Attribute = call.GetCustomAttribute<GameConsoleCommandAttribute>();
            ExampleGetter = exampleGetter;
            ExampleAttribute = exampleGetter?.GetCustomAttribute<GameConsoleCommandExampleAttribute>();

            var ps = Call.GetParameters();
            if (ps.Length > 0)
            {
                Parameter = ps[0];
            }
        }

        public void Send(string argText)
        {
            var parameters = Call.GetParameters();
            if (parameters.Length == 1)
            {
                var p = parameters[0];
                var send = _methodOfInternalSendWithArg.MakeGenericMethod(p.ParameterType);

                try
                {
                    send.Invoke(this, new object[] { Call, argText });
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException != null)
                        throw e.InnerException;
                    throw;
                }
            }
            else
            {
                InternalSendNoArg(Call, argText);
            }
        }

        public string GetExample()
        {
            if (ExampleGetter != null)
            {
                return (string)ExampleGetter.Invoke(null, null);
            }
            return null;
        }

        private void InternalSendWithArg<T>(MethodInfo call, string argText)
        {
            if (argText.IsNullOrWhiteSpace() && !Attribute.OptionalParameter)
            {
                throw new ArgumentException($"The command '{Attribute.Name}' argument cannot be empty!");
            }

            object arg = null;
            if (typeof(T).IsPrimitive)
            {
                if (typeof(T).IsIntegerType())
                    arg = argText.ToInt();
                else if (typeof(T).IsFloatingPointType())
                    arg = argText.ToFloat();
                else
                    throw new NotImplementedException($"The arg type '{typeof(T)}' is not implemented!");
            }
            else if (typeof(T).IsStringType())
                arg = argText;
            else
            {
                var data = new EasySerializationData(argText, EasyDataFormat.Json);
                arg = EasySerialize.From<T>(ref data);
            }

            call.Invoke(this, new object[] { arg });
        }

        private void InternalSendNoArg(MethodInfo call, string argText)
        {
            call.Invoke(this, null);
        }
    }
}
