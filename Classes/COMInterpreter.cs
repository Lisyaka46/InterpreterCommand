using Interpreter.Classes;
using Interpreter.Commands;
using Interpreter.Interfaces;
using InterpreterCommand.Commands;
using InterpreterCommand.Interfaices;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Buffer = Interpreter.Classes.Buffer;

namespace InterpreterCommand.Classes
{
    public class COMInterpreter<TViewer> : COMInterpreterBase where TViewer : ICommandViewer
    {
        private Dictionary<string, CommandOPER<TViewer>> MainCommand;
        /// <summary>
        /// Массив команд доступных по ключу для использования интерпретатором
        /// </summary>
        public ReadOnlyDictionary<string, CommandOPER<TViewer>> Commands => MainCommand.AsReadOnly();

        private Dictionary<string, AliasCommand<CommandOPER<TViewer>, TViewer>> MainAliasCommand;
        /// <summary>
        /// Массив алиасов доступных по ключу для использования интерпретатором
        /// </summary>
        public ReadOnlyDictionary<string, AliasCommand<CommandOPER<TViewer>, TViewer>> Aliases => MainAliasCommand.AsReadOnly();

        /// <summary>
        /// Количество алиасов
        /// </summary>
        public int AliasesCount => MainAliasCommand.Count;

        /// <summary>
        /// Количество команд
        /// </summary>
        public int CommandCount => MainCommand.Count;

        /// <summary>
        /// Получить команду по её имени
        /// </summary>
        /// <param name="NameCommand">Имя команды</param>
        /// <returns>Возможно найденная команда</returns>
        public CommandOPER<TViewer>? GetCommandFindName(string NameCommand) =>
            MainCommand.TryGetValue(NameCommand, out CommandOPER<TViewer>? value) ? value : null;

        /// <summary>
        /// Добавить команду для использования интерпретатором
        /// </summary>
        /// <param name="Command">Добавляемая команда</param>
        /// <returns>Состояние, добавилась или нет команда</returns>
        public bool AddCommand(CommandOPER<TViewer> Command) => MainCommand.TryAdd(Command.Name, Command);

        /// <summary>
        /// Добавить алиас на команду
        /// </summary>
        /// <param name="Command">Добавляемая команда</param>
        /// <returns>Состояние, добавилась или нет команда</returns>
        public bool AddAliasCommand(string Name, string Command, string Description)
        {
            AliasCommand<CommandOPER<TViewer>, TViewer> alias = new(Name, Command, Description, ReadCommand(Command));
            return MainAliasCommand.TryAdd(alias.Name, alias);
        }

        /// <summary>
        /// Перевести все элементы команд интерпретатора по определённому условному выделению элементов
        /// </summary>
        /// <typeparam name="TResult">Возвращаемый тип выделяемых свойств</typeparam>
        /// <param name="selector">Функция выделения</param>
        /// <returns></returns>
        public IEnumerable<ICommandOPER<TViewer>> CommandWhere(Func<ICommandOPER<TViewer>, bool> selector)
        {
            List<ICommandOPER<TViewer>> results = [];
            results.AddRange(MainCommand.Values.Where(selector));
            results.AddRange(MainAliasCommand.Values.Where(selector));
            return [.. results];
        }

        /// <summary>
        /// Создать пустой объект интерпретатора
        /// </summary>
        public COMInterpreter()
        {
            MainCommand = [];
            MainAliasCommand = [];
        }

        /// <summary>
        /// Создать объект интерпретатора с пользовательскими командами
        /// </summary>
        /// <param name="commands">Добавляемый массив используемых команд</param>
        public COMInterpreter(IEnumerable<CommandOPER<TViewer>> commands)
        {
            MainCommand = new Dictionary<string, CommandOPER<TViewer>>(commands.ToDictionary(x => x.Name, x => x));
            MainAliasCommand = [];
        }

        /// <summary>
        /// Найти команду любого содержания
        /// </summary>
        /// <param name="TextCommand">Читаемая команда</param>
        public CommandOPER<TViewer>? ReadCommand(string TextCommand)
        {
            string Command = ReadNameCommand(TextCommand);
            if (Command[^1] == '*') Command = Command[..^1];
            MainCommand.TryGetValue(Command, out CommandOPER<TViewer>? MainCommandIntepreter);
            if (MainCommandIntepreter == null)
            {
                MainAliasCommand.TryGetValue(Command, out AliasCommand<CommandOPER<TViewer>, TViewer>? AliasCommandIntepreter);
                return AliasCommandIntepreter;
            }
            return MainCommandIntepreter;
        }

        /// <summary>
        /// Найти команду определённого типа<br/>
        /// <b>Не ищет алиасы</b>
        /// </summary>
        /// <param name="TextCommand">Читаемая команда</param>
        public T? ReadCommand<T>(string TextCommand) where T : CommandOPER<TViewer>
        {
            MainCommand.TryGetValue(ReadNameCommand(TextCommand), out CommandOPER<TViewer>? CommandIntepreter);
            return CommandIntepreter?.GetType() == typeof(T) ? (T)CommandIntepreter : default;
        }

        /// <summary>
        /// Найти алиас команды
        /// </summary>
        /// <param name="TextCommand">Читаемая команда алиаса</param>
        public AliasCommand<CommandOPER<TViewer>, TViewer>? ReadAliasCommand(string TextCommand) =>
            MainAliasCommand.TryGetValue(ReadNameCommand(TextCommand), out AliasCommand<CommandOPER<TViewer>, TViewer>? SourceCommand) ? SourceCommand : null;

        /// <summary>
        /// Прочитать и выполнить команду
        /// </summary>
        /// <param name="BufferCommand">Класс буфера команд</param>
        /// <param name="TextCommand">Читаемая команда</param>
        /// <param name="CommandViewer">Объект визуализирующий команду</param>
        public async Task<CommandStateResult> ReadAndExecuteCommand(Buffer? BufferCommand, string TextCommand, TViewer? CommandViewer)
        {
            string NameCommand = ReadNameCommand(TextCommand);
            BufferCommand?.Add(NameCommand);
            bool CompleteSearch = MainCommand.TryGetValue(NameCommand, out CommandOPER<TViewer>? Command);
            if (Command == null)
            {
                CompleteSearch = MainAliasCommand.TryGetValue(NameCommand, out AliasCommand<CommandOPER<TViewer>, TViewer>? Alias);
                Command = Alias;
            }
            if (!CompleteSearch || Command == null) return CommandStateResult.FaledCommand(NameCommand);
            string[] Parameters = ReadParametersCommand(TextCommand);
            try
            {
                return await Command.ExecuteCommand(Parameters, CommandViewer);
            }
            catch
            {
                return await Command.ExecuteCommand(CommandViewer);
            }
        }
    }
}
