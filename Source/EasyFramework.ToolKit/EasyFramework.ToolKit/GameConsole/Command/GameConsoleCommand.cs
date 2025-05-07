using EasyFramework.Serialization;
using System.Reflection;
using System.Text;
using System;
using EasyFramework.Core;

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

            object arg;
            var type = typeof(T);
            if (type.IsBasic())
            {
                if (type.IsInteger())
                    arg = argText.ToInt();
                else if (type.IsFloatingPoint())
                    arg = argText.ToFloat();
                else if (type.IsString())
                    arg = argText;
                else if (type.IsBoolean())
                    arg = argText == "True";
                else
                    throw new NotImplementedException($"The arg type '{typeof(T)}' is not implemented!");
            }
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
