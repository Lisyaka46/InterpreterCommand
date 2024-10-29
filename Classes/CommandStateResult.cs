
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
    public class CommandStateResult
    {
        /// <summary>
        /// Итоговое состояние команды
        /// </summary>
        public readonly ResultState State;

        /// <summary>
        /// Сообщение в LOG
        /// </summary>
        public readonly string LOGMassage;

        /// <summary>
        /// Сообщение в консольную строку
        /// </summary>
        public readonly string Massage;

        /// <summary>
        /// Имя выполняющейся команды
        /// </summary>
        public readonly string NameCommand;

        /// <summary>
        /// Успешный итог выполнения команды
        /// </summary>
        /// <param name="NameCommand">Имя команды</param>
        public static CommandStateResult Completed(string NameCommand) => new(ResultState.Complete, NameCommand, string.Empty, string.Empty);

        /// <summary>
        /// Ошибочный итог выполнения команды
        /// </summary>
        /// /// <param name="Message">Текст ошибки выводимый в консоль программу</param>
        /// <param name="Message_log">Текст который будет записан в лог программы</param>
        /// <param name="Command">Имя команды</param>
        public static CommandStateResult Failed(string Command, string Message, string Message_log) => new(ResultState.Failed, Command, Message, Message_log);

        /// <summary>
        /// Ошибочный итог выполнения команды
        /// </summary>
        /// /// <param name="Message">Текст ошибки выводимый в консоль программу</param>
        /// <param name="Message_log">Текст который будет записан в лог программы</param>
        /// <param name="Command">Имя команды</param>
        public static CommandStateResult Failed(string Command, string Message) => new(ResultState.Failed, Command, Message);

        /// <summary>
        /// Ошибочный итог выполнения команды из-за недостатка параметров
        /// </summary>
        /// <param name="NameCommand">Имя команды которая привела к ошибке</param>
        public static CommandStateResult FaledParameteres(string NameCommand)
        {
            return new(ResultState.InvalidParameters, NameCommand, $"Команда \"{NameCommand}\" привела к ошибке из-за недостатка параметров.");
        }

        /// <summary>
        /// Ошибочный итог выполнения команды из-за типа параметров
        /// </summary>
        /// <param name="NameCommand">Имя команды которая привела к ошибке</param>
        /// <param name="NumberParam">Номер неправильного парметра</param>
        public static CommandStateResult FaledTypeParameteres(string NameCommand, int NumberParam)
        {
            return new(ResultState.InvalidParameters, NameCommand, $"Команда \"{NameCommand}\" привела к ошибке из-за не правильного предоставления типа параметра №{NumberParam}.");
        }

        /// <summary>
        /// Ошибочный итог выполнения команды из-за несуществующей команды
        /// </summary>
        /// <param name="NameCommand">Имя команды которая привела к ошибке</param>
        public static CommandStateResult FaledCommand(string NameCommand)
        {
            return new(ResultState.InvalidCommand, NameCommand, $"Команда \"{NameCommand}\" не найдена.");
        }

        /// <summary>
        /// Инициализировать объект итога выполнения команды
        /// </summary>
        /// <param name="ResultState">Итоговое состояние выполнения</param>
        /// <param name="Massage">Сообщение в консольную строку</param>
        /// <param name="Massage_log">Сообщение в LOG</param>
        /// <param name="Namу">Имя команды</param>
        private CommandStateResult(ResultState ResultState, string Name, string Massage, string Massage_log)
        {
            NameCommand = Name;
            State = ResultState;
            this.Massage = Massage;
            LOGMassage = Massage_log;
        }

        /// <summary>
        /// Инициализировать объект итога выполнения команды
        /// </summary>
        /// <param name="ResultState">Итоговое состояние выполнения</param>
        /// <param name="Massage">Сообщение в консольную строку</param>
        /// <param name="Name">Имя команды</param>
        private CommandStateResult(ResultState ResultState, string Name, string Massage)
        {
            NameCommand = Name;
            State = ResultState;
            this.Massage = Massage;
            LOGMassage = string.Empty;
        }
    }
}
