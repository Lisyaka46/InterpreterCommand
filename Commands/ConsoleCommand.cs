using Interpreter.Classes;
using Interpreter.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Interpreter.Commands
{
    /// <summary>
    /// Консольная команда
    /// </summary>
    public partial class ConsoleCommand : ICommandOPER
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
        /// Делегат события выполнения команды
        /// </summary>
        /// <param name="MainCommand">Выполняющаяся команда</param>
        /// <param name="ParametersValue">Параметры команды</param>
        /// <returns>Итог выполнения команды</returns>
        public delegate Task<CommandStateResult> ExecuteCom(ICommandOPER MainCommand, object[] ParametersValue);

        /// <summary>
        /// Действие которое выполняет команда
        /// </summary>
        internal event ExecuteCom Execute;

        /// <summary>
        /// Инициализировать объект консольной команды с параметрами
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Parameters">Параметры команды</param>
        /// <param name="Description">Описание команды</param>
        /// <param name="Execute">Действие выполнения</param>
        public ConsoleCommand(string Name, Parameter[] Parameters, string Description, ExecuteCom Execute)
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
        public ConsoleCommand(string Name, string Description, ExecuteCom Execute)
        {
            this.Name = Name;
            this.Description = Description;
            this.Execute = Execute;
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
        /// <param name="parameters">Параметры команды</param>
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

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        public CommandStateResult ExecuteCommand()
        {
            if (Parameters == null) return Execute.Invoke(this, []).Result;
            return CommandStateResult.FaledParameteres(Name);
        }
    }
}
