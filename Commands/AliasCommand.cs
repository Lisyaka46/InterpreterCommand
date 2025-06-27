using Interpreter.Classes;
using Interpreter.Interfaces;
using InterpreterCommand.Classes;

namespace Interpreter.Commands
{
    /// <summary>
    /// Создать алиас на любую команду
    /// </summary>
    /// <param name="Name">Имя команды</param>
    /// <param name="Command">Строка команды выполнения</param>
    /// <param name="Description">Описание алиаса</param>
    /// <param name="Magnite">Команда привязки</param>
    public class AliasCommand<T> : ICommandOPER where T : ICommandOPER
    {
        /// <summary>
        /// Имя команды
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Команда которую выполняет алиас
        /// </summary>
        public string NameCommand { get; private set; }

        /// <summary>
        /// Параметры записанные в команду
        /// </summary>
        public string[] ParametersCommand { get; private set; }

        /// <summary>
        /// Описание алиаса
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Команда к которой ссылается алиас
        /// </summary>
        public T? SourceCommand { get; private set; }

        /// <summary>
        /// Параметры алиаса
        /// </summary>
        public Parameter[] Parameters { get; private set; } = [];

        public AliasCommand(string name, string command, string description, T? Magnite)
        {
            Name = name;
            NameCommand = COMInterpreter.ReadNameCommand(command);
            ParametersCommand = COMInterpreter.ReadParametersCommand(command);
            Description = description;
            SourceCommand = Magnite;
            ChangeAbsolutlyParam(Magnite);
        }

        /// <summary>
        /// Изменить команду на которую ссылается алиас
        /// </summary>
        /// <param name="TextCommand">Текст новой ссылки на команду</param>
        /// <returns></returns>
        public CommandStateResult ChangeSourceCommand(T? Command, string TextCommand, string? description)
        {
            string name = COMInterpreter.ReadNameCommand(TextCommand);
            if (Command == null) return CommandStateResult.FaledCommand(name);
            NameCommand = name;
            SourceCommand = Command;
            ParametersCommand = COMInterpreter.ReadParametersCommand(TextCommand);
            ChangeAbsolutlyParam(Command);
            if (description != null) Description = description;
            return CommandStateResult.Completed(NameCommand);
        }

        private void ChangeAbsolutlyParam(T? CommandMagnite)
        {
            Parameter[] AbsolutlyParamCommand = CommandMagnite?.Parameters ?? [];
            if (ParametersCommand.Length == 0) Parameters = AbsolutlyParamCommand;
            else Parameters = ParametersCommand.Length < AbsolutlyParamCommand.Length ? AbsolutlyParamCommand[ParametersCommand.Length..] : [];
        }

        /// <summary>
        /// Выполнить действие алиаса
        /// </summary>
        /// <param name="DataCommand">Массив данных команд</param>
        /// <returns>Результат выполнения</returns>
        public CommandStateResult ExecuteCommand() => SourceCommand?.ExecuteCommand() ?? CommandStateResult.FaledCommand(COMInterpreter.ReadNameCommand(NameCommand));

        /// <summary>
        /// Выполнить действие алиаса
        /// </summary>
        /// <param name="parameters">Параметры команды</param>
        /// <returns>Результат выполнения</returns>
        public CommandStateResult ExecuteCommand(string[] parameters)
        {
            if (SourceCommand == null) return CommandStateResult.FaledCommand(COMInterpreter.ReadNameCommand(NameCommand));
            string[] MainParam = new string[parameters.Length + ParametersCommand.Length];
            if (ParametersCommand.Length > 0) ParametersCommand.CopyTo(MainParam, 0);
            if (parameters.Length > 0) parameters.CopyTo(MainParam, ParametersCommand.Length);
            return SourceCommand.ExecuteCommand(MainParam);
        }
    }
}
