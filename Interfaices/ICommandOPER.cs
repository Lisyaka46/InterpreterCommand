using Interpreter.Classes;
using InterpreterCommand.Interfaices;

namespace Interpreter.Interfaces
{
    public partial interface ICommandOPER<TViewer> where TViewer : ICommandViewer
    {
        /// <summary>
        /// Делегат события выполнения команды
        /// </summary>
        /// <param name="MainCommand">Выполняющаяся команда</param>
        /// <param name="ParametersValue">Параметры команды</param>
        /// <returns>Итог выполнения команды</returns>
        public delegate Task<CommandStateResult> ExecuteCom(ICommandOPER<TViewer> MainCommand);

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
        /// Описание команды
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Параметры команды
        /// </summary>
        public Parameter[]? Parameters { get; }

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        /// <param name="CommandViewer">Объект визуализирующий команду</param>
        internal Task<CommandStateResult> ExecuteCommand(TViewer? CommandViewer);

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        /// <param name="Param">Параметры для выполнения команды</param>
        /// <param name="CommandViewer">Объект визуализирующий команду</param>
        protected internal Task<CommandStateResult> ExecuteCommand(string[] Param, TViewer? CommandViewer);
    }
}
