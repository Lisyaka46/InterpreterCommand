using Interpreter.Classes;

namespace Interpreter.Interfaces
{
    public partial interface ICommandOPER
    {
        /// <summary>
        /// Делегат события выполнения команды
        /// </summary>
        /// <param name="MainCommand">Выполняющаяся команда</param>
        /// <param name="ParametersValue">Параметры команды</param>
        /// <returns>Итог выполнения команды</returns>
        public delegate Task<CommandStateResult> ExecuteCom(ICommandOPER MainCommand);

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
        internal CommandStateResult ExecuteCommand();

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        protected internal CommandStateResult ExecuteCommand(string[] parameters);
    }
}
