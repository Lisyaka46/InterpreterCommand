namespace Interpreter.Classes
{
    /// <summary>
    /// Конечные результаты выполнения команды
    /// </summary>
    public enum ResultState
    {
        /// <summary>
        /// Ошибка различия параметров
        /// </summary>
        InvalidParameters = -2,

        /// <summary>
        /// Команда не распознана
        /// </summary>
        InvalidCommand = -1,

        /// <summary>
        /// Команда не выполнилась
        /// </summary>
        Failed = 0,

        /// <summary>
        /// Команда выполнилась успешно
        /// </summary>
        Complete = 1
    }

    /// <summary>
    /// Объект итогового состояния выполнения команды
    /// </summary>
    public readonly struct CommandStateResult
    {
        /// <summary>
        /// Итоговое состояние команды
        /// </summary>
        public readonly ResultState State;

        /// <summary>
        /// Программное сообщение
        /// </summary>
        public readonly string? MessageLog;

        /// <summary>
        /// Сообщение
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Имя выполняющейся команды
        /// </summary>
        public readonly string NameCommand;

        /// <summary>
        /// Инициализировать объект итога выполнения команды
        /// </summary>
        /// <param name="ResultState">Итоговое состояние выполнения</param>
        /// <param name="Name">Имя команды</param>
        /// <param name="Message">Сообщение итога</param>
        /// <param name="MessageLog">Программное сообщение</param>
        private CommandStateResult(ResultState ResultState, string Name, string Message, string? MessageLog = null)
        {
            NameCommand = Name;
            State = ResultState;
            this.Message = Message;
            this.MessageLog = MessageLog ?? string.Empty;
        }

        /// <summary>
        /// Успешный итог выполнения команды
        /// </summary>
        /// <param name="NameCommand">Имя команды</param>
        /// <param name="Message">Сообщение итога</param>
        /// <param name="MessageLog">Программное сообщение</param>
        public static CommandStateResult Completed(string NameCommand, string? Message = null, string? MessageLog = null) =>
            new(ResultState.Complete, NameCommand, Message ?? string.Empty, MessageLog);

        /// <summary>
        /// Ошибочный итог выполнения команды
        /// </summary>
        /// <param name="Command">Имя команды</param>
        /// <param name="Message">Текст ошибки выводимый в консоль программу</param>
        /// <param name="MessageLog">Текст который будет записан в лог программы</param>
        public static CommandStateResult Failed(string Command, string Message, string? MessageLog = null) =>
            new(ResultState.Failed, Command, Message, MessageLog);

        /// <summary>
        /// Ошибочный итог выполнения команды из-за недостатка параметров
        /// </summary>
        /// <param name="NameCommand">Имя команды которая привела к ошибке</param>
        public static CommandStateResult FaledParameteres(string NameCommand) =>
            new(ResultState.InvalidParameters, NameCommand, $"Команда \"{NameCommand}\" привела к ошибке из-за недостатка параметров.");

        /// <summary>
        /// Ошибочный итог выполнения команды из-за типа параметров
        /// </summary>
        /// <param name="NameCommand">Имя команды которая привела к ошибке</param>
        /// <param name="NumberParam">Номер неправильного парметра</param>
        public static CommandStateResult FaledTypeParameteres(string NameCommand, int NumberParam) =>
            new(ResultState.InvalidParameters, NameCommand, $"Команда \"{NameCommand}\" привела к ошибке из-за не правильного предоставления типа параметра №{NumberParam}.");

        /// <summary>
        /// Ошибочный итог выполнения команды из-за несуществующей команды
        /// </summary>
        /// <param name="NameCommand">Имя команды которая привела к ошибке</param>
        public static CommandStateResult FaledCommand(string NameCommand) =>
            new(ResultState.InvalidCommand, NameCommand, $"Команда \"{NameCommand}\" не найдена.");
    }
}
