using Interpreter.Classes;
using Interpreter.Interfaces;
using InterpreterCommand.Interfaices;
using System.Diagnostics.CodeAnalysis;

namespace Interpreter.Commands
{
    /// <summary>
    /// Консольная команда
    /// </summary>
    public partial class ConsoleCommand<TViewer> : ICommandOPER<TViewer> where TViewer : ICommandViewer
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
        /// <param name="CommandViewer">Объект визуализирующий команду</param>
        /// <returns>Итог выполнения команды</returns>
        public delegate Task<CommandStateResult> ExecuteCom(ICommandOPER<TViewer> MainCommand, object[] ParametersValue, TViewer? CommandViewer);

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
        /// <param name="Param">Параметры команды</param>
        /// <param name="CommandViewer">Объект визуализирующий команду</param>
        public async Task<CommandStateResult> ExecuteCommand(string[] Param, TViewer? CommandViewer)
        {
            if (!AbsolutlyRequiredParameters(Param)) return CommandStateResult.FaledParameteres(Name);
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
                                MainParameters[i] = Convert.ToInt32(Param[i]);
                            }
                            catch (FormatException) { return CommandStateResult.FaledTypeParameteres(Name, i + 1); }
                        }
                        else if (Parameters[i].TypeP == typeof(bool))
                        {
                            if (Param[i].ToLower().Equals("true") || Param[i].Equals("1")) MainParameters[i] = true;
                            else if (Param[i].ToLower().Equals("false") || Param[i].Equals("0")) MainParameters[i] = false;
                            else return CommandStateResult.FaledTypeParameteres(Name, i + 1);
                        }
                        else MainParameters[i] = Param[i];
                    }
                    catch (IndexOutOfRangeException) { MainParameters[i] = Parameters[i].DefValue ?? string.Empty; }
                }
            }
            return await Execute.Invoke(this, MainParameters, CommandViewer);
        }

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        /// <param name="CommandViewer">Объект визуализирующий команду</param>
        public async Task<CommandStateResult> ExecuteCommand(TViewer? CommandViewer)
        {
            if (Parameters == null) return await Execute.Invoke(this, [], CommandViewer);
            return CommandStateResult.FaledParameteres(Name);
        }
    }
}
