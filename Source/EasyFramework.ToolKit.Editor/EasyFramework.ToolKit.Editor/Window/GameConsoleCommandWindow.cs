using System;
using System.Collections;
using System.Linq;
using EasyFramework.Editor;
using EasyFramework.Serialization;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class GameConsoleCommandWindow : OdinEditorWindow
    {
        [MenuItem("Tools/EasyFramework/GameConsole Commands")]
        public static void ShowWindow()
        {
            var isNew = HasOpenInstances<GameConsoleCommandWindow>();
            var window = GetWindow<GameConsoleCommandWindow>("GameConsole Commands");
            if (!isNew)
            {
                window.CenterWindowWithSizeRadio(new Vector2(0.4f, 0.45f));
            }
        }

        [HideLabel, InlineProperty, Serializable]
        public class CommandInfo
        {
            public GameConsoleCommand TargetCommand { get; set; }

            [LabelText("描述")]
            [TextArea(1, 3)]
            public string Description;

            [LabelText("可选参数")]
            [ReadOnly]
            [ShowIf(nameof(HasParameter))]
            public bool OptionalParameter;

            [LabelText("参数类型")]
            [ReadOnly]
            [ShowIf(nameof(HasParameter))]
            public string ParameterType;

            [LabelText("示例")]
            [TextArea(1, 3)]
            public string Example;

            public bool HasParameter => TargetCommand.Parameter != null;
        }

        [HideLabel, InlineProperty, Serializable, HideReferenceObjectPicker]
        public class BuildConfig
        {
            public GameConsoleCommand TargetCommand { get; set; }

            [ShowIf(nameof(OptionalParameter))]
            [LabelText("构建参数")]
            public bool BuildArgument;

            [HideReferenceObjectPicker]
            [ShowIf(nameof(ShowArgument))]
            [LabelText("参数")]
            public object Argument;

            [LabelText("输出命令")]
            [TextArea(1, 3)]
            public string Output;

            private bool OptionalParameter => TargetCommand != null && TargetCommand.Attribute.OptionalParameter;
            private bool ShowArgument => BuildArgument && TargetCommand.Parameter != null;

            [Button]
            public void Build()
            {
                Output = TargetCommand.Attribute.Name;
                if (Argument != null && BuildArgument)
                {
                    Output += " ";
                    var paramType = TargetCommand.Parameter.ParameterType;
                    if (paramType.IsValueType || paramType == typeof(string))
                    {
                        Output += Argument.ToString();
                    }
                    else
                    {
                        var data = new EasySerializationData(EasyDataFormat.Json);
                        EasySerialize.To(Argument, paramType, ref data);
                        Output += data.StringData;
                    }
                }
            }
        }

        [ValueDropdown(nameof(GetCommandsDropdown))]
        [OnValueChanged(nameof(OnCommandChanged))]
        [LabelText("命令")]
        public string Command;

        [FoldoutGroup("信息")]
        [ShowIf("@_command != null")]
        public CommandInfo Info = new CommandInfo();

        [FoldoutGroup("构建")]
        [ShowIf("@_command != null")]
        [NonSerialized, OdinSerialize]
        public BuildConfig Build = new BuildConfig();

        private GameConsoleCommand _command;

        private ValueDropdownList<string> _commandsDropdown;

        IEnumerable GetCommandsDropdown()
        {
            if (_commandsDropdown == null)
            {
                _commandsDropdown = new ValueDropdownList<string>();
                _commandsDropdown.AddRange(GameConsoleCommandsManager.Instance.GetCommands()
                    .Select(cmd => new ValueDropdownItem<string>(
                            (cmd.IsSystem ? "system/" : "custom/") + cmd.Attribute.Name,
                            cmd.Attribute.Name)));
            }

            return _commandsDropdown;
        }

        void OnValidate()
        {
            _command = GameConsoleCommandsManager.Instance.GetCommand(Command);
            Info.TargetCommand = _command;
            Build.TargetCommand = _command;
        }

        void OnCommandChanged()
        {
            _command = GameConsoleCommandsManager.Instance.GetCommand(Command);
            Info.TargetCommand = _command;
            Build.TargetCommand = _command;
            if (_command == null)
                return;

            Info.Description = _command.Attribute.Description;

            if (_command.Parameter != null)
            {
                var paramType = _command.Parameter.ParameterType;
                if (paramType.IsPrimitive || paramType.IsStringType())
                {
                    Info.ParameterType = paramType.GetAliases();
                }
                else
                {
                    Info.ParameterType = "Object";
                }

                Info.OptionalParameter = _command.Attribute.OptionalParameter;
            }

            Info.Example = _command.GetExample();

            if (_command.Parameter != null)
            {
                Build.Argument = _command.Parameter.ParameterType.CreateInstance();
            }
            else
            {
                Build.Argument = null;
            }

            Build.Output = string.Empty;
        }
    }
}
