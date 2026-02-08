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
                Match Name = RegexNameCommand().Match(TextCommand);
                string ParametersString = ClearAnyOneReplySymbol(TextCommand[(Name.Length + 1)..], ' ');
                if (RegexAnyOneSymbolParameter().Count(ParametersString) == 0) return Parameters;
                else if (ParametersString[0] is ' ') ParametersString = ParametersString[1..];

                Parameters = [..
                    RegexSortParamCommand().Matches(ParametersString).Select((i) => i.Value) ];
                for (int i = 0; i < Parameters.Length; i++)
                {
                    if (Parameters[i][0] is ' ' or '~')
                        Parameters[i] = Parameters[i][1..];
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
        public static string ReadNameCommand(string TextCommand) =>
            ClearAllReplySymbol(RegexNameCommand().Match(TextCommand).Value, ' ').Replace(' ', '_').ToLower();

        /// <summary>
        /// Удаление повторяющихся символов (bbbabbb) => (bab)
        /// </summary>
        /// <param name="Text">Текст</param>
        /// <param name="Symbol">символ поиска в тексте</param>
        /// <returns>Очищенный текст от повторяющегося символа</returns>
        private static string ClearAllReplySymbol(string Text, char Symbol)
        {
            foreach (Match m in Regex.Matches(Text, Symbol + @"{2,}"))
            {
                Text = Text.Replace(m.Value, Symbol.ToString());
            }
            return Text;
        }

        /// <summary>
        /// Удаление повторяющихся символов <b>Только в начале строки</b> (bbbabbb) => (babbb)
        /// </summary>
        /// <param name="Text">Текст</param>
        /// <param name="Symbol">символ поиска в тексте</param>
        /// <returns>Очищенный текст от повторяющегося символа</returns>
        private static string ClearAnyOneReplySymbol(string Text, char Symbol)
        {
            foreach (Match m in Regex.Matches(Text, $"^{Symbol}" + @"{2,}"))
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
        /// Регулярное выражение сортировки параметров от специальных символов
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        [GeneratedRegex(@"([^%,]+|%,|%%)+")]
        private static partial Regex RegexSortParamCommand();

        /// <summary>
        /// Регулярное выражение не пробела
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        [GeneratedRegex(@"[^ ]")]
        private static partial Regex RegexAnyOneSymbolParameter();
    }
}
