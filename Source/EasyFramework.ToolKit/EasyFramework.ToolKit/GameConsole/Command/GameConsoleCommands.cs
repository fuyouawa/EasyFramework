using System.Linq;
using System.Text;

namespace EasyFramework.ToolKit
{
    static class GameConsoleCommands
    {
        [GameConsoleCommand("help", Description = "Use 'help <command>' for more information. (such as 'help echo')")]
        static void Help(string command)
        {
            if (command.IsNotNullOrWhiteSpace())
            {
                var cmd = GameConsoleCommandsManager.Instance.GetCommand(command);
                if (cmd == null)
                {
                    GameConsole.Instance.LogError($"Unknown command: {command}");
                    return;
                }

            }
            else
            {
                var cmds = GameConsoleCommandsManager.Instance.GetCommands();

                var sb = new StringBuilder();
                sb.AppendLine("------------ Commands Document ------------");

                var systemCmds = cmds.Where(c => c.IsSystem).ToArray();
                if (systemCmds.Length > 0)
                {
                    sb.AppendLine("System:");
                    sb.Append(CommandsString(systemCmds));
                }

                var customCmds = cmds.Where(c => !c.IsSystem).ToArray();
                if (customCmds.Length > 0)
                {
                    sb.AppendLine("Custom:");
                    sb.Append(CommandsString(customCmds));
                }

                GameConsole.Instance.LogInfo(sb.ToString());
            }

            static string CommandsString(GameConsoleCommandAttribute[] cmds)
            {
                return string.Join("\r\n", cmds.Select(c => $"\t{c.Name,-15}{c.Description}"));
            }
        }
    }
}
