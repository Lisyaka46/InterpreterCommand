using Interpreter.Classes;
using Interpreter.Interfaces;
using InterpreterCommand.Classes;
using InterpreterCommand.Commands;
using InterpreterCommand.Interfaices;

namespace Interpreter.Commands
{
    /// <summary>
    /// Класс алиаса команды<br/>
    /// <see href="TCommand" cref=" Тип хранящейся команды в алиасе"/><br/>
    /// <see href="TViewer" cref=" Тип визуализационного объекта команды"/>
    /// </summary>
    public class AliasCommand<TCommand, TViewer> : CommandOPER<TViewer> where TCommand : CommandOPER<TViewer> where TViewer : ICommandViewer
    {
        /// <summary>
        /// Команда которую выполняет алиас
        /// </summary>
        public string NameCommand { get; private set; }

        /// <summary>
        /// Параметры записанные в команду
        /// </summary>
        public string[] ParametersCommand { get; private set; }

        /// <summary>
        /// Команда к которой ссылается алиас
        /// </summary>
        public TCommand? SourceCommand { get; private set; }

        /// <summary>
        /// Инициализация алиаса
        /// </summary>
        /// <param name="Name">Имя команды</param>
        /// <param name="Command">Строка команды выполнения</param>
        /// <param name="Description">Описание алиаса</param>
        /// <param name="Magnite">Команда привязки</param>
        public AliasCommand(string Name, string Command, string Description, TCommand? Magnite)
        {
            this.Name = Name;
            NameCommand = COMInterpreterBase.ReadNameCommand(Command);
            ParametersCommand = COMInterpreterBase.ReadParametersCommand(Command);
            this.Description = Description;
            SourceCommand = Magnite;
            ChangeAbsolutlyParam(Magnite);
        }

        /// <summary>
        /// Изменить команду на которую ссылается алиас
        /// </summary>
        /// <param name="TextCommand">Текст новой ссылки на команду</param>
        /// <returns></returns>
        public CommandStateResult ChangeSourceCommand(TCommand? Command, string TextCommand, string? description)
        {
            string name = COMInterpreterBase.ReadNameCommand(TextCommand);
            if (Command == null) return CommandStateResult.FaledCommand(name);
            NameCommand = name;
            SourceCommand = Command;
            ParametersCommand = COMInterpreterBase.ReadParametersCommand(TextCommand);
            ChangeAbsolutlyParam(Command);
            if (description != null) Description = description;
            return CommandStateResult.Completed(NameCommand);
        }

        private void ChangeAbsolutlyParam(TCommand? CommandMagnite)
        {
            Parameter[] AbsolutlyParamCommand = CommandMagnite?.Parameters ?? [];
            if (ParametersCommand.Length == 0) Parameters = AbsolutlyParamCommand;
            else Parameters = ParametersCommand.Length < AbsolutlyParamCommand.Length ? AbsolutlyParamCommand[ParametersCommand.Length..] : [];
        }

        /// <summary>
        /// Выполнить действие алиаса
        /// </summary>
        /// <param name="CommandViewer">Объект визуализирующий команду</param>
        /// <returns>Результат выполнения</returns>
        public new async Task<CommandStateResult> ExecuteCommand(TViewer? CommandViewer) =>
            SourceCommand != null ? await SourceCommand.ExecuteCommand(CommandViewer) :
            CommandStateResult.FaledCommand(COMInterpreterBase.ReadNameCommand(NameCommand));

        /// <summary>
        /// Выполнить действие алиаса
        /// </summary>
        /// <param name="Param">Параметры команды</param>
        /// <param name="CommandViewer">Объект визуализирующий команду</param>
        /// <returns>Результат выполнения</returns>
        public new async Task<CommandStateResult> ExecuteCommand(string[] Param, TViewer? CommandViewer)
        {
            if (SourceCommand == null) return CommandStateResult.FaledCommand(COMInterpreterBase.ReadNameCommand(NameCommand));
            string[] MainParam = new string[Param.Length + ParametersCommand.Length];
            if (ParametersCommand.Length > 0) ParametersCommand.CopyTo(MainParam, 0);
            if (Param.Length > 0) Param.CopyTo(MainParam, ParametersCommand.Length);
            return await SourceCommand.ExecuteCommand(MainParam, CommandViewer);
        }
    }
}
