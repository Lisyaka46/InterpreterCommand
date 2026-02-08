using Interpreter.Classes;
using Interpreter.Interfaces;
using InterpreterCommand.Commands;
using InterpreterCommand.Interfaices;
using System.Diagnostics.CodeAnalysis;

namespace Interpreter.Commands
{
    /// <summary>
    /// Консольная команда
    /// </summary>
    public partial class ConsoleCommand<TViewer> : CommandOPER<TViewer> where TViewer : ICommandViewer
    {
        /// <summary>
        /// Инициализировать объект консольной команды с параметрами
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Parameters">Параметры команды</param>
        /// <param name="Description">Описание команды</param>
        /// <param name="Execute">Действие выполнения</param>
        public ConsoleCommand(string Name, Parameter[] Parameters, string Description, ExecuteCom Execute)
        {
            base.Name = Name;
            base.Description = Description;
            base.Execute = Execute;
            base.Parameters = Parameters;

        }

        /// <summary>
        /// Инициализировать объект консольной команды без параметров
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Description">Описание команды</param>
        /// <param name="Execute">Действие выполнения</param>
        public ConsoleCommand(string Name, string Description, ExecuteCom Execute)
        {
            base.Name = Name;
            base.Description = Description;
            base.Execute = Execute;
            Parameters = [];

        }

        ///// <summary>
        ///// Инициализировать объект консольной команды без параметров
        ///// </summary>
        ///// <param name="Name">Имя</param>
        ///// <param name="Description">Описание команды</param>
        ///// <param name="Execute">Действие выполнения</param>
        //public ConsoleCommand(string Name, string Description, ExecuteCom Execute)
        //{
        //    base.Name = Name;
        //    base.Description = Description;
        //    base.Execute = Execute;
        //}

        ///// <summary>
        ///// Создать выполнение команды
        ///// </summary>
        ///// <param name="Param">Параметры команды</param>
        ///// <param name="CommandViewer">Объект визуализирующий команду</param>
        //public async Task<CommandStateResult> ExecuteCommand(string[] Param, TViewer? CommandViewer)
        //{
        //    if (!AbsolutlyRequiredParameters(Param)) return CommandStateResult.FaledParameteres(Name);
        //    object[] MainParameters = [];
        //    if (Parameters != null)
        //    {
        //        MainParameters = new object[Parameters.Length];
        //        for (int i = 0; i < Parameters.Length; i++)
        //        {
        //            try
        //            {
        //                if (Parameters[i].TypeP == typeof(int))
        //                {
        //                    try
        //                    {
        //                        MainParameters[i] = Convert.ToInt32(Param[i]);
        //                    }
        //                    catch (FormatException) { return CommandStateResult.FaledTypeParameteres(Name, i + 1); }
        //                }
        //                else if (Parameters[i].TypeP == typeof(bool))
        //                {
        //                    if (Param[i].ToLower().Equals("true") || Param[i].Equals("1")) MainParameters[i] = true;
        //                    else if (Param[i].ToLower().Equals("false") || Param[i].Equals("0")) MainParameters[i] = false;
        //                    else return CommandStateResult.FaledTypeParameteres(Name, i + 1);
        //                }
        //                else MainParameters[i] = Param[i];
        //            }
        //            catch (IndexOutOfRangeException)
        //            {
        //                MainParameters[i] = Parameters[i].DefValue ?? string.Empty;
        //            }
        //        }
        //    }
        //    CommandStateResult Result = await Execute.Invoke(this, MainParameters, CommandViewer);
        //    if (While)
        //    return await Execute.Invoke(this, MainParameters, CommandViewer);
        //}

        ///// <summary>
        ///// Создать выполнение команды
        ///// </summary>
        ///// <param name="CommandViewer">Объект визуализирующий команду</param>
        //public async Task<CommandStateResult> ExecuteCommand(TViewer? CommandViewer)
        //{
        //    if (Parameters == null) return await Execute.Invoke(this, [], CommandViewer);
        //    return CommandStateResult.FaledParameteres(Name);
        //}
    }
}
