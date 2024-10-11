using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Interpreter.Commands
{
    /// <summary>
    /// Консольная команда
    /// </summary>
    public partial class ConsoleCommand : ICommandAAC
    {
        /// <summary>
        /// Имя команды
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Описание консольной команды
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Параметры команды
        /// </summary>
        [AllowNull()]
        public Parameter[] Parameters { get; private set; }

        /// <summary>
        /// Действие которое выполняет команда
        /// </summary>
        private event ICommandAAC.ExecuteCom Execute;

        /// <summary>
        /// Инициализировать объект консольной команды с параметрами
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Parameters">Параметры команды</param>
        /// <param name="Description">Описание команды</param>
        /// <param name="Execute">Действие выполнения</param>
        public ConsoleCommand(string Name, Parameter[] Parameters, string Description, ICommandAAC.ExecuteCom Execute)
        {
            this.Name = Name;
            this.Parameters = Parameters;
            this.Description = Description;
            this.Execute = Execute;

        }

        /// <summary>
        /// Инициализировать объект консольной команды без параметров
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Description">Описание команды</param>
        /// <param name="Execute">Действие выполнения</param>
        public ConsoleCommand(string Name, string Description, ICommandAAC.ExecuteCom Execute)
        {
            this.Name = Name;
            this.Description = Description;
            this.Execute = Execute;
        }

        /// <summary>
        /// Прочитать и выполнить команду
        /// </summary>
        /// <param name="ConsoleCommands">Массив поиска консольных команд</param>
        /// <param name="TextCommand">Читаемая команда</param>
        public static CommandStateResult ReadAndExecuteCommand(Classes.Buffer? BufferCommand,
            [NotNull()] ConsoleCommand[] ConsoleCommands, string TextCommand)
        {
            string NameCommand = ReadNameCommand(TextCommand);
            ConsoleCommand? SearchCommand = ConsoleCommands.SingleOrDefault(i => i.Name.Equals(NameCommand));
            BufferCommand?.Add(NameCommand);
            if (SearchCommand == null) return CommandStateResult.FaledCommand(NameCommand);
            else
            {
                string[] Parameters = ReadParametersCommand(TextCommand);
                return SearchCommand.ExecuteCommand(Parameters);
            }
        }

        /// <summary>
        /// Найти команду
        /// </summary>
        /// <param name="ConsoleCommands">Массив поиска консольных команд</param>
        /// <param name="TextCommand">Читаемая команда</param>
        public static ConsoleCommand? ReadCommand([NotNull()] ConsoleCommand[] ConsoleCommands, string TextCommand)
        {
            string NameCommand = ReadNameCommand(TextCommand);
            return ConsoleCommands.SingleOrDefault(i => i.Name.Equals(NameCommand));
        }

        /// <summary>
        /// Прочитать имя команды
        /// </summary>
        /// <param name="TextCommand">Читаемая команда</param>
        public static string ReadNameCommand(string TextCommand)
        {
            string Name;
            if (TextCommand.Contains('*')) // command * param1, param2, param3 ...
                Name = ClearReplySymbol(ICommandAAC.RegexNameCommand().Match(TextCommand).Value, ' ');
            else // command
                Name = ClearReplySymbol(TextCommand, ' ');
            return Name.Replace(' ', '_').ToLower();
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
                    ICommandAAC.RegexSortParamCommand().Matches(
                        ICommandAAC.RegexParameterCommand().Match(TextCommand).Value[1..])
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
        private static string ClearReplySymbol(string Text, char Symbol)
        {
            foreach (Match m in Regex.Matches(Text, Symbol + @"{2,}"))
            {
                Text = Text.Replace(m.Value, Symbol.ToString());
            }
            return Text;
        }

        /// <summary>
        /// Узнать написаны ли обязательные параметры команды
        /// </summary>
        /// <param name="WritingParameters">Написанные параметры</param>
        /// <returns>Совпадает правилу или нет</returns>
        public bool AbsolutlyRequiredParameters(string[] WritingParameters)
        {
            if (Parameters == null) return true;
            else return WritingParameters.Length >= Parameters.Count((i) => i.Absolutly == true);
        }

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        public CommandStateResult ExecuteCommand(string[] parameters)
        {
            if (!AbsolutlyRequiredParameters(parameters)) return CommandStateResult.FaledParameteres(Name);
            object[] MainParameters = [];
            if (Parameters != null)
            {
                MainParameters = new object[Parameters.Length];
                for (int i = 0; i < Parameters.Length; i++)
                {
                    try
                    {
                        if (Parameters[i].TypeP == typeof(int))
                        {
                            try
                            {
                                MainParameters[i] = Convert.ToInt32(parameters[i]);
                            }
                            catch (FormatException) { return CommandStateResult.FaledTypeParameteres(Name, i + 1); }
                        }
                        else if (Parameters[i].TypeP == typeof(bool))
                        {
                            if (parameters[i].ToLower().Equals("true") || parameters[i].Equals("1")) MainParameters[i] = true;
                            else if (parameters[i].ToLower().Equals("false") || parameters[i].Equals("0")) MainParameters[i] = false;
                            else return CommandStateResult.FaledTypeParameteres(Name, i + 1);
                        }
                        else MainParameters[i] = parameters[i];
                    }
                    catch (IndexOutOfRangeException) { MainParameters[i] = Parameters[i].DefValue ?? string.Empty; }
                }
            }
            return Execute.Invoke(this, MainParameters).Result;
        }
    }
}
