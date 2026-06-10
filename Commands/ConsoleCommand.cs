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
    public sealed partial class ConsoleCommand<TViewer> : CommandOPER<TViewer> where TViewer : ICommandViewer
    {
        /// <summary>
        /// Инициализировать объект консольной команды с параметрами
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Parameters">Параметры команды</param>
        /// <param name="Description">Описание команды</param>
        /// <param name="Execute">Действие выполнения</param>
        public ConsoleCommand(CommandLevel SourceLevel, string Name, Parameter[] Parameters, string Description, ExecuteCom Execute)
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
        public ConsoleCommand(CommandLevel SourceLevel, string Name, string Description, ExecuteCom Execute)
        {
            base.Name = Name;
            base.Description = Description;
            base.Execute = Execute;
            Parameters = [];

        }
    }
}
