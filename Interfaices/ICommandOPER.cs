using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Interpreter.Classes;
using Interpreter.Commands;

namespace Interpreter.Interfaces
{
    public partial interface ICommandOPER
    {
        /// <summary>
        /// Делегат события выполнения команды
        /// </summary>
        /// <param name="MainCommand">Выполняющаяся команда</param>
        /// <param name="ParametersValue">Параметры команды</param>
        /// <returns>Итог выполнения команды</returns>
        public delegate Task<CommandStateResult> ExecuteCom(ICommandOPER MainCommand);

        /// <summary>
        /// Действие которое выполняет команда
        /// </summary>
        internal event ExecuteCom Execute
        {
            add => Execute += value;
            remove => Execute -= value;
        }

        /// <summary>
        /// Название команды
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Параметры команды
        /// </summary>
        public Parameter[]? Parameters { get; }

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        internal CommandStateResult ExecuteCommand();

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        protected internal CommandStateResult ExecuteCommand(string[] parameters);

        /// <summary>
        /// Прочитать и выполнить команду
        /// </summary>
        /// <param name="Commands">Массив поиска консольных команд</param>
        /// <param name="TextCommand">Читаемая команда</param>
        public sealed static CommandStateResult ReadAndExecuteCommand<T>(Classes.Buffer? BufferCommand,
            [NotNull()] T[] Commands, string TextCommand) where T : ICommandOPER
        {
            string NameCommand = ReadNameCommand(TextCommand);
            T? SearchCommand = Commands.SingleOrDefault(i => i.Name.Equals(NameCommand));
            BufferCommand?.Add(NameCommand);
            if (SearchCommand == null) return CommandStateResult.FaledCommand(NameCommand);
            else
            {
                string[] Parameters = ReadParametersCommand(TextCommand);
                try
                {
                    return SearchCommand.ExecuteCommand(Parameters);
                }
                catch
                {
                    return SearchCommand.ExecuteCommand();
                }
            }
        }

        /// <summary>
        /// Найти команду
        /// </summary>
        /// <param name="Commands">Массив поиска команд</param>
        /// <param name="TextCommand">Читаемая команда</param>
        public sealed static T? ReadCommand<T>([NotNull()] T[] Commands, string TextCommand) where T : ICommandOPER
        {
            string NameCommand = ReadNameCommand(TextCommand);
            return Commands.SingleOrDefault(i => i.Name.Equals(NameCommand));
        }

        /// <summary>
        /// Прочитать имя команды
        /// </summary>
        /// <param name="TextCommand">Читаемая команда</param>
        public sealed static string ReadNameCommand(string TextCommand)
        {
            string Name;
            if (TextCommand.Contains('*')) // command * param1, param2, param3 ...
                Name = ClearReplySymbol(RegexNameCommand().Match(TextCommand).Value, ' ');
            else // command
                Name = ClearReplySymbol(TextCommand, ' ');
            return Name.Replace(' ', '_').ToLower();
        }

        /// <summary>
        /// Прочитать параметры команды
        /// </summary>
        /// <param name="TextCommand">Читаемая команда</param>
        public sealed static string[] ReadParametersCommand(string TextCommand)
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
        /// Удаление повторяющихся символов
        /// </summary>
        /// <param name="Text">Текст</param>
        /// <param name="Symbol">символ поиска в тексте</param>
        /// <returns>Очищенный текст от повторяющегося символа</returns>
        private sealed static string ClearReplySymbol(string Text, char Symbol)
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
        internal sealed static partial Regex RegexNameCommand();

        /// <summary>
        /// Регулярное выражение сортировки имени и параметров команды
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        [GeneratedRegex(@"\*.*")]
        internal sealed static partial Regex RegexParameterCommand();

        /// <summary>
        /// Регулярное выражение сортировки параметров от специальных символов
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        [GeneratedRegex(@"([^%,]+|%,|%%)+")]
        internal sealed static partial Regex RegexSortParamCommand();
    }
}
