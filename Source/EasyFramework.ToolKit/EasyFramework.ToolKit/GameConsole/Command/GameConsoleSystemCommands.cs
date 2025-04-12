using System.Linq;
using System.Text;

namespace EasyFramework.ToolKit
{
    static class GameConsoleSystemCommands
    {
        [GameConsoleCommand("help", OptionalParameter = true, Description = "Use 'help <command>' for more information. (such as 'help echo')")]
        static void Help(string command)
        {
            const int align = 15;

            if (command.IsNotNullOrWhiteSpace())
            {
                var cmd = GameConsoleCommandsManager.Instance.GetCommand(command);
                if (cmd == null)
                {
                    GameConsole.Instance.LogError($"Unknown command: {command}");
                    return;
                }
                
                var sb = new StringBuilder();
                sb.AppendLine("------------ Command Information ------------");
                sb.AppendLine($"Command: {command}");
                sb.AppendLine("Description:");
                sb.AppendLine($"\t{cmd.Attribute.Description}");

                if (cmd.Parameter != null)
                {
                    sb.AppendLine("Argument:");
                    sb.Append("\t" + "Optional".PadRight(align));
                    if (cmd.Attribute.OptionalParameter)
                    {
                        sb.AppendLine("True");
                    }
                    else
                    {
                        sb.AppendLine("False");
                    }

                    sb.Append("\t" + "Type".PadRight(align));

                    var paramType = cmd.Parameter.ParameterType;
                    if (paramType.IsPrimitive || paramType.IsStringType())
                    {
                        sb.AppendLine(paramType.GetAliases());
                    }
                    else
                    {
                        sb.AppendLine("Object");
                    }
                }
                
                sb.AppendLine("Example:");
                var example = cmd.GetExample();
                if (example.IsNotNullOrWhiteSpace())
                {
                    sb.Append($"\t{example}");
                }
                else
                {
                    sb.Append("Empty example.");
                }

                GameConsole.Instance.LogInfo(sb.ToString());
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

            static string CommandsString(GameConsoleCommand[] cmds)
            {
                return string.Join("\r\n", cmds.Select(c => $"\t{c.Attribute.Name,-align}{c.Attribute.Description}"));
            }
        }

        [GameConsoleCommandExample("help")]
        static string HelpExample()
        {
            return "help echo";
        }

        [GameConsoleCommand("echo", Description = "Echo the input argument.")]
        static void Echo(string message)
        {
            GameConsole.Instance.LogInfo(message);
        }

        [GameConsoleCommandExample("echo")]
        static string EchoExample()
        {
            return "echo Hello world!";
        }

        [GameConsoleCommand("clear", Description = "Clear log.")]
        static void Clear()
        {
            GameConsole.Instance.ClearLogs();
        }

        [GameConsoleCommandExample("clear")]
        static string ClearExample()
        {
            return "clear";
        }
    }
}
