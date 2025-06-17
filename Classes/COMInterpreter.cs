using Interpreter.Classes;
using Interpreter.Commands;
using Interpreter.Interfaces;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Buffer = Interpreter.Classes.Buffer;

namespace InterpreterCommand.Classes
{
    public partial class COMInterpreter
    {
        private Dictionary<string, ICommandOPER> MainCommand;
        /// <summary>
        /// Массив команд доступных по ключу для использования интерпретатором
        /// </summary>
        public ReadOnlyDictionary<string, ICommandOPER> Commands => MainCommand.AsReadOnly();

        private Dictionary<string, AliasCommand<ICommandOPER>> MainAliasCommand;
        /// <summary>
        /// Массив алиасов доступных по ключу для использования интерпретатором
        /// </summary>
        public ReadOnlyDictionary<string, AliasCommand<ICommandOPER>> Aliases => MainAliasCommand.AsReadOnly();

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
        public ICommandOPER? GetCommandFindName(string NameCommand) => MainCommand.TryGetValue(NameCommand, out ICommandOPER? value) ? value : null;

        /// <summary>
        /// Добавить команду для использования интерпретатором
        /// </summary>
        /// <param name="Command">Добавляемая команда</param>
        /// <returns>Состояние, добавилась или нет команда</returns>
        public bool AddCommand(ICommandOPER Command) => MainCommand.TryAdd(Command.Name, Command);

        /// <summary>
        /// Добавить алиас на команду
        /// </summary>
        /// <param name="Command">Добавляемая команда</param>
        /// <returns>Состояние, добавилась или нет команда</returns>
        public bool AddAliasCommand(string Name, string Command, string Description)
        {
            AliasCommand<ICommandOPER> alias = new(Name, Command, Description, ReadCommand(Command));
            return MainAliasCommand.TryAdd(alias.Name, alias);
        }

        /// <summary>
        /// Перевести все элементы команд интерпретатора по определённому условному выделению элементов
        /// </summary>
        /// <typeparam name="TResult">Возвращаемый тип выделяемых свойств</typeparam>
        /// <param name="selector">Функция выделения</param>
        /// <returns></returns>
        public IEnumerable<ICommandOPER> CommandWhere(Func<ICommandOPER, bool> selector)
        {
            List<ICommandOPER> results = [];
            results.AddRange(MainCommand.Values.Where(selector));
            results.AddRange(MainAliasCommand.Values.Where(selector));
            return [..results];
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
        public COMInterpreter(IEnumerable<ICommandOPER> commands)
        {
            MainCommand = new Dictionary<string, ICommandOPER>(commands.ToDictionary(x => x.Name, x => x));
            MainAliasCommand = [];
        }

        /// <summary>
        /// Найти команду
        /// </summary>
        /// <param name="TextCommand">Читаемая команда</param>
        public ICommandOPER? ReadCommand(string TextCommand)
        {
            MainCommand.TryGetValue(ReadNameCommand(TextCommand), out ICommandOPER? Result);
            if (Result == null) MainCommand.TryGetValue(ReadNameCommand(TextCommand), out Result);
            return Result;
        }

        /// <summary>
        /// Найти алиас команды
        /// </summary>
        /// <param name="TextCommand">Читаемая команда алиаса</param>
        public AliasCommand<ICommandOPER>? ReadAliasCommand(string TextCommand) =>
            MainAliasCommand.TryGetValue(ReadNameCommand(TextCommand), out AliasCommand<ICommandOPER>? SourceCommand) ? SourceCommand : null;

        /// <summary>
        /// Прочитать и выполнить команду
        /// </summary>
        /// <param name="BufferCommand">Класс буфера команд</param>
        /// <param name="TextCommand">Читаемая команда</param>
        public CommandStateResult ReadAndExecuteCommand(Buffer? BufferCommand, string TextCommand)
        {
            string NameCommand = ReadNameCommand(TextCommand);
            BufferCommand?.Add(NameCommand);
            bool CompleteSearch = MainCommand.TryGetValue(NameCommand, out ICommandOPER? Command);
            if (Command == null)
            {
                CompleteSearch = MainAliasCommand.TryGetValue(NameCommand, out AliasCommand<ICommandOPER>? Alias);
                Command = Alias;
            }
            if (!CompleteSearch || Command == null) return CommandStateResult.FaledCommand(NameCommand);
            string[] Parameters = ReadParametersCommand(TextCommand);
            try
            {
                return Command.ExecuteCommand(Parameters);
            }
            catch
            {
                return Command.ExecuteCommand();
            }
        }

        /// <summary>
        /// Прочитать параметры команды
        /// </summary>
        /// <param name="TextCommand">Читаемая команда</param>
        public static string[] ReadParametersCommand(string TextCommand)
        {
            string[] Parameters = []; // command
            if (TextCommand.Contains('*')) // command * param1, param2, param3 ...
            {
                Parameters = [..
                    RegexSortParamCommand().Matches(
                        RegexParameterCommand().Match(TextCommand).Value[1..])
                    .Select((i) => i.Value) ];
                for (int i = 0; i < Parameters.Length; i++)
                {
                    switch (Parameters[i][0])
                    {
                        case ' ':
                        case '~':
                            Parameters[i] = Parameters[i][1..];
                            break;
                    }
                    Parameters[i] = Parameters[i].Replace("%,", ",");
                    Parameters[i] = Parameters[i].Replace("%%", "%");
                }
            }
            return Parameters;
        }

        /// <summary>
        /// Прочитать имя команды
        /// </summary>
        /// <param name="TextCommand">Читаемая команда</param>
        public static string ReadNameCommand(string TextCommand)
        {
            string Name;
            if (TextCommand.Contains('*')) // command * param1, param2, param3 ...
                Name = ClearReplySymbol(RegexNameCommand().Match(TextCommand).Value, ' ');
            else // command
                Name = ClearReplySymbol(TextCommand, ' ');
            return Name.Replace(' ', '_').ToLower();
        }

        /// <summary>
        /// Удаление повторяющихся символов
        /// </summary>
        /// <param name="Text">Текст</param>
        /// <param name="Symbol">символ поиска в тексте</param>
        /// <returns>Очищенный текст от повторяющегося символа</returns>
        private static string ClearReplySymbol(string Text, char Symbol)
        {
            foreach (Match m in Regex.Matches(Text, Symbol + @"{2,}"))
            {
                Text = Text.Replace(m.Value, Symbol.ToString());
            }
            return Text;
        }

        /// <summary>
        /// Регулярное выражение имени команды
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        [GeneratedRegex(@"[^*]+")]
        private static partial Regex RegexNameCommand();

        /// <summary>
        /// Регулярное выражение сортировки имени и параметров команды
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        [GeneratedRegex(@"\*.*")]
        private static partial Regex RegexParameterCommand();

        /// <summary>
        /// Регулярное выражение сортировки параметров от специальных символов
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        [GeneratedRegex(@"([^%,]+|%,|%%)+")]
        private static partial Regex RegexSortParamCommand();
    }
}
