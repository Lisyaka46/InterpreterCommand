using Interpreter.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpreter.Interfaces;
using System.Diagnostics.CodeAnalysis;
using Interpreter.Classes;

namespace Interpreter.Commands
{
    /// <summary>
    /// Создать алеас на любую команду
    /// </summary>
    /// <param name="Name">Имя команды</param>
    /// <param name="Command">Строка команды выполнения</param>
    /// <param name="Execute">Действие выполнения команды</param>
    /// <param name="CommandsMagnite">Массив команд к которым ссылается алеас</param>
    public class AliasCommand<T>(string Name, string Command, T[] CommandsMagnite) : ICommandAAC where T : ICommandAAC 
    {
        /// <summary>
        /// Имя команды
        /// </summary>
        public string Name { get; } = Name;

        /// <summary>
        /// Команда которую выполняет алеас
        /// </summary>
        public string Command { get; set; } = Command;

        /// <summary>
        /// Действие которое выполняет команда
        /// </summary>
        internal ICommandAAC? CommandObject = ICommandAAC.ReadCommand([.. CommandsMagnite], Command);

        /// <summary>
        /// Параметры команды
        /// </summary>
        public Parameter[]? Parameters { get; }

        /// <summary>
        /// Выполнить действие алеаса
        /// </summary>
        /// <returns>Результат выполнения</returns>
        public CommandStateResult ExecuteCommand() => 
            CommandObject?.ExecuteCommand(ICommandAAC.ReadParametersCommand(Command)) ?? CommandStateResult.FaledCommand(Name);

        /// <summary>
        /// Выполнить действие алеаса
        /// </summary>
        /// <param name="parameters">Параметры команды</param>
        /// <returns>Результат выполнения</returns>
        public CommandStateResult ExecuteCommand(string[] parameters) =>
            CommandObject?.ExecuteCommand(parameters) ?? CommandStateResult.FaledCommand(Name);
    }
}
