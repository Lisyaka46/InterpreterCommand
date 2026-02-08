using Interpreter.Classes;
using InterpreterCommand.Interfaices;

namespace Interpreter.Interfaces
{
    public partial interface ICommandOPER<TViewer> where TViewer : ICommandViewer
    {
        /// <summary>
        /// Название команды
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Описание команды
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Параметры команды
        /// </summary>
        public Parameter[]? Parameters { get; }
    }
}
