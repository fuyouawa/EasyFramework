using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyFramework.ToolKit
{
    internal class GameConsoleCommandsManager : Singleton<GameConsoleCommandsManager>
    {
        private readonly Dictionary<string, GameConsoleCommand>
            _commands = new Dictionary<string, GameConsoleCommand>();

        GameConsoleCommandsManager()
        {
        }

        protected override void OnSingletonInit()
        {
            var methods = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .SelectMany(t => t.GetMethods(BindingFlagsHelper.AllStatic()));

            var calls = methods
                .Where(m => m.HasCustomAttribute<GameConsoleCommandAttribute>() && m.GetParameters().Length <= 1);

            var examples = methods
                .Where(m => m.HasCustomAttribute<GameConsoleCommandExampleAttribute>() &&
                            m.ReturnParameter != null &&
                            m.ReturnParameter.ParameterType == typeof(string))
                .ToDictionary(m => m.GetCustomAttribute<GameConsoleCommandExampleAttribute>().Name, m => m);

            foreach (var call in calls)
            {
                var attr = call.GetCustomAttribute<GameConsoleCommandAttribute>();
                _commands[attr.Name] = new GameConsoleCommand(call, examples.GetValueOrDefault(attr.Name));
            }
        }

        public GameConsoleCommand[] GetCommands()
        {
            return _commands.Values.ToArray();
        }

        public GameConsoleCommand GetCommand(string name)
        {
            return _commands.GetValueOrDefault(name.Trim());
        }
    }
}
