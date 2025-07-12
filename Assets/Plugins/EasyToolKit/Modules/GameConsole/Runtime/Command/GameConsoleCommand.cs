using System.Reflection;
using System.Text;
using System;
using EasyToolKit.Core;
using EasyToolKit.ThirdParty.OdinSerializer;

namespace EasyToolKit.GameConsole
{
    public class GameConsoleCommand
    {
        public MethodInfo Call { get; }
        public MethodInfo ExampleGetter { get; }
        public GameConsoleCommandAttribute Attribute { get; }
        public GameConsoleCommandExampleAttribute ExampleAttribute { get; }

        public ParameterInfo Parameter { get; }

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

                try
                {
                    InternalSendWithArg(p.ParameterType, Call, argText);
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

        private void InternalSendWithArg(Type argType, MethodInfo call, string argText)
        {
            if (argText.IsNullOrWhiteSpace() && !Attribute.OptionalParameter)
            {
                throw new ArgumentException($"Command '{Attribute.Name}' argument cannot be empty!");
            }

            object arg;
            if (argType.IsBasic())
            {
                if (argType.IsInteger())
                    arg = argText.ToInt();
                else if (argType.IsFloatingPoint())
                    arg = argText.ToFloat();
                else if (argType.IsString())
                    arg = argText;
                else if (argType.IsBoolean())
                    arg = argText == "True";
                else
                    throw new NotImplementedException($"Argument type of '{argType}' is not implemented!");
            }
            else
            {
                arg = SerializationUtility.DeserializeValueWeak(Encoding.UTF8.GetBytes(argText), DataFormat.JSON);
            }

            call.Invoke(this, new object[] { arg });
        }

        private void InternalSendNoArg(MethodInfo call, string argText)
        {
            call.Invoke(this, null);
        }
    }
}
