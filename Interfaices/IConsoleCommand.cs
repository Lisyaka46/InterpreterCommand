using Interpreter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpreter.Classes;

namespace Interpreter.Interfaces
{
    public interface IConsoleCommand : ICommandAAC
    {
        /// <summary>
        /// Делегат события выполнения команды
        /// </summary>
        /// <param name="MainCommand">Выполняющаяся команда</param>
        /// <param name="ParametersValue">Параметры команды</param>
        /// <returns>Итог выполнения команды</returns>
        public new delegate Task<CommandStateResult> ExecuteCom(ICommandAAC MainCommand, object[] ParametersValue);

        /// <summary>
        /// Действие которое выполняет команда
        /// </summary>
        internal new event ExecuteCom Execute
        {
            add => Execute += value;
            remove => Execute -= value;
        }

        /// <summary>
        /// Описание команды
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Узнать написаны ли обязательные параметры команды
        /// </summary>
        /// <param name="WritingParameters">Написанные параметры</param>
        /// <returns>Совпадает правилу или нет</returns>
        protected abstract bool AbsolutlyRequiredParameters(string[] WritingParameters);
    }
}
