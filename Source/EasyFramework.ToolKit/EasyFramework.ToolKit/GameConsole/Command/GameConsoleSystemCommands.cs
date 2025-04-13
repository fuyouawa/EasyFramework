using System.Linq;
using System.Text;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    static class GameConsoleSystemCommands
    {
        [GameConsoleCommand("help", OptionalParameter = true, Description = "使用 help <command> 获取指令的更多信息。（例如 help echo）")]
        static void Help(string command)
        {
            const int align = 30;

            if (command.IsNotNullOrWhiteSpace())
            {
                var cmd = GameConsoleCommandsManager.Instance.GetCommand(command);
                if (cmd == null)
                {
                    GameConsole.Instance.LogError($"Unknown command: {command}");
                    Debug.Log($"Unknown command: {command}");
                    return;
                }
                
                var sb = new StringBuilder();
                sb.AppendLine("------------ 指令信息 ------------");
                sb.AppendLine("描述：");
                sb.AppendLine($"\t{cmd.Attribute.Description}");

                if (cmd.Parameter != null)
                {
                    sb.AppendLine("参数：");
                    sb.Append("\t" + "可选".PadRight(align));
                    if (cmd.Attribute.OptionalParameter)
                    {
                        sb.AppendLine("是");
                    }
                    else
                    {
                        sb.AppendLine("否");
                    }

                    sb.Append("\t" + "类型".PadRight(align));

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
                
                sb.AppendLine("示例:");
                var example = cmd.GetExample();
                if (example.IsNotNullOrWhiteSpace())
                {
                    sb.Append($"\t{example}");
                }
                else
                {
                    sb.Append("空。");
                }

                GameConsole.Instance.LogInfo(sb.ToString());
            }
            else
            {
                var cmds = GameConsoleCommandsManager.Instance.GetCommands();

                var sb = new StringBuilder();
                sb.AppendLine("------------ 指令文档 ------------");

                var systemCmds = cmds.Where(c => c.IsSystem).ToArray();
                if (systemCmds.Length > 0)
                {
                    sb.AppendLine("系统指令：");
                    sb.Append(CommandsString(systemCmds));
                }

                var customCmds = cmds.Where(c => !c.IsSystem).ToArray();
                if (customCmds.Length > 0)
                {
                    sb.AppendLine("自定义指令：");
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

        [GameConsoleCommand("echo", Description = "回应输入的参数。")]
        static void Echo(string message)
        {
            GameConsole.Instance.LogInfo(message);
        }

        [GameConsoleCommandExample("echo")]
        static string EchoExample()
        {
            return "echo Hello world!";
        }

        [GameConsoleCommand("clear", Description = "清空控制台的日志。")]
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
