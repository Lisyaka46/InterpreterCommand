using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;

namespace AAC20.Classes.Commands
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
        public static CommandStateResult ReadAndExecuteCommand(Buffer? BufferCommand,
            [NotNull()] ConsoleCommand[] ConsoleCommands, string TextCommand)
        {
            string[] Parameters = [];
            string Name;
            if (TextCommand.Contains('*')) // command * param1, param2, param3 ...
            {
                Name = ClearReplySymbol(ICommandAAC.RegexNameCommand().Match(TextCommand).Value, ' ');
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
            else // command
            {
                Name = ClearReplySymbol(TextCommand, ' ');
            }
            Name = Name.Replace(" ", "_").ToLower();
            ConsoleCommand? SearchCommand = ConsoleCommands.SingleOrDefault(i => i.Name.Equals(Name));
            BufferCommand?.Add(Name);
            if (SearchCommand == null) return CommandStateResult.FaledCommand(Name);
            else
            {
                return SearchCommand.AbsolutlyRequiredParameters(Parameters) ?
                    SearchCommand.ExecuteCommand(Parameters) : CommandStateResult.FaledParameteres(SearchCommand.Name);
            }
        }

        //
        private static string ClearReplySymbol(string Text, char Symbol)
        {
            Text = new([.. Text.Reverse()]);
            for (int i = 0, count = 0; i < Text.Length; i++)
            {
                if (Text[i] == Symbol) count = i + 1;
                else return new([.. Text.Remove(0, count).Reverse()]);
            }
            return new([.. Text.Reverse()]);
        }

        /// <summary>
        /// Узнать написаны ли обязательные параметры команды
        /// </summary>
        /// <param name="WritingParameters">Написанные параметры</param>
        /// <returns>Совпадает правилу или нет</returns>
        public bool AbsolutlyRequiredParameters(string[] WritingParameters)
        {
            if (Parameters == null) return true;
            else
            {
                int Count = Parameters.Count((i) => i.Absolutly == true);
                if (WritingParameters.Length >= Parameters.Count((i) => i.Absolutly == true))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        public CommandStateResult ExecuteCommand(string[] parameters)
        {
            object[] MainParameters = [];
            if (Parameters != null)
            {
                MainParameters = new object[Parameters.Length];
                for (int i = 0; i < Parameters.Length; i++)
                {
                    if (Parameters[i].TypeP == typeof(int))
                    {
                        try
                        {
                            MainParameters[i] = Convert.ToInt32(parameters[i]);
                        }
                        catch { return CommandStateResult.FaledTypeParameteres(Name); }
                    }
                }
            }
            return Execute.Invoke(this, MainParameters).Result;
        }
    }
}
