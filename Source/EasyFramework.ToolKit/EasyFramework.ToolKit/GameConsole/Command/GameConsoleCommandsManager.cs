using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EasyFramework.Serialization;

namespace EasyFramework.ToolKit
{
    internal class GameConsoleCommandsManager : Singleton<GameConsoleCommandsManager>
    {
        private readonly Dictionary<string, MethodInfo> _commands = new Dictionary<string, MethodInfo>();

        private MethodInfo _internalSendWithArgMethod;

        GameConsoleCommandsManager()
        {
        }

        protected override void OnSingletonInit()
        {
            var methods = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .SelectMany(t => t.GetMethods(BindingFlagsHelper.AllStatic()))
                .Where(m => m.HasCustomAttribute<GameConsoleCommandAttribute>() && m.GetParameters().Length <= 1)
                .ToArray();

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<GameConsoleCommandAttribute>();
                _commands[attr.Name] = method;
            }

            _internalSendWithArgMethod =
                GetType().GetMethod(nameof(InternalSendWithArg), BindingFlagsHelper.NonPublicInstance());
        }

        public GameConsoleCommandAttribute[] GetCommands()
        {
            return _commands.Values
                .Select(GetAttribute)
                .ToArray();
        }

        public GameConsoleCommandAttribute GetCommand(string name)
        {
            if (_commands.TryGetValue(name.Trim(), out var call))
            {
                return GetAttribute(call);
            }

            return null;
        }

        private GameConsoleCommandAttribute GetAttribute(MethodInfo call)
        {
            var attr = call.GetCustomAttribute<GameConsoleCommandAttribute>();
            attr.IsSystem = call.DeclaringType == typeof(GameConsoleCommands);
            return attr;
        }

        public void Send(string command, string argJson)
        {
            if (!_commands.TryGetValue(command, out var call))
            {
                GameConsole.Instance.Log(GameConsole.LogType.Error, $"Unknown command: {command}");
                return;
            }

            try
            {
                var parameters = call.GetParameters();
                if (parameters.Length == 1)
                {
                    var p = parameters[0];
                    var send = _internalSendWithArgMethod.MakeGenericMethod(p.ParameterType);
                    send.Invoke(this, new object[] { command, call, argJson });
                }
                else
                {
                    InternalSendNoArg(command, call, argJson);
                }
            }
            catch (Exception e)
            {
                GameConsole.Instance.Log(GameConsole.LogType.Error, $"Send command failed: {e.Message}");
                return;
            }
        }

        private void InternalSendWithArg<T>(string command, MethodInfo call, string argText)
        {
            if (argText.IsNotNullOrWhiteSpace())
            {
                object arg = null;
                if (typeof(T).IsPrimitive)
                {
                    if (typeof(T).IsIntegerType())
                        arg = argText.ToInt();
                    else if (typeof(T).IsFloatingPointType())
                        arg = argText.ToFloat();
                    else if (typeof(T).IsStringType())
                        arg = argText;
                    else
                        throw new NotImplementedException($"The arg type '{typeof(T)}' is not implemented!");
                }
                else
                {
                    arg = EasySerialize.From<T>(EasyDataFormat.Json, Encoding.UTF8.GetBytes(argText));
                }

                call.Invoke(this, new object[] { arg });
            }
            else
            {
                call.Invoke(this, new object[] { null });
            }
        }

        private void InternalSendNoArg(string command, MethodInfo call, string argJson)
        {
            call.Invoke(this, null);
        }
    }
}
