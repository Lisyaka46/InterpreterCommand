using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InterpreterCommand.Classes
{
    /// <summary>
    /// Базовый абстрактный класс реализации интерпретатора
    /// </summary>
    public abstract partial class COMInterpreterBase
    {
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
