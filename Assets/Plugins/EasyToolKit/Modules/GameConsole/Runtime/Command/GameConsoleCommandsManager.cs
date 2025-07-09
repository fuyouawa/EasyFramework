using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyToolKit.Core;

namespace EasyToolKit.GameConsole
{
    public class GameConsoleCommandsManager : Singleton<GameConsoleCommandsManager>
    {
        private readonly Dictionary<string, GameConsoleCommand>
            _commandsByName = new Dictionary<string, GameConsoleCommand>();

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
                            m.ReturnType == typeof(string))
                .ToDictionary(m => m.GetCustomAttribute<GameConsoleCommandExampleAttribute>().Name, m => m);

            foreach (var call in calls)
            {
                var attr = call.GetCustomAttribute<GameConsoleCommandAttribute>();
                var example = examples.GetValueOrDefault(attr.Name);
                _commandsByName[attr.Name] = new GameConsoleCommand(call, example);
            }
        }

        public GameConsoleCommand[] GetCommands()
        {
            return _commandsByName.Values.ToArray();
        }

        public GameConsoleCommand GetCommand(string name)
        {
            return _commandsByName.GetValueOrDefault(name.Trim());
        }
    }
}
