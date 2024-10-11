using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Interpreter.Commands
{
    public partial interface ICommandAAC
    {
        /// <summary>
        /// Делегат события выполнения команды
        /// </summary>
        /// <param name="MainCommand">Выполняющаяся команда</param>
        /// <param name="ParametersValue">Параметры команды</param>
        /// <returns>Итог выполнения команды</returns>
        public delegate Task<CommandStateResult> ExecuteCom(ICommandAAC MainCommand, object[] ParametersValue);

        /// <summary>
        /// Действие которое выполняет команда
        /// </summary>
        internal event ExecuteCom Execute
        {
            add => Execute += value;
            remove => Execute -= value;
        }

        /// <summary>
        /// Параметры команды
        /// </summary>
        public Parameter[]? Parameters { get; }

        /// <summary>
        /// Название команды
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Описание команды
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        protected CommandStateResult ExecuteCommand(string[] parameters);

        /// <summary>
        /// Узнать написаны ли обязательные параметры команды
        /// </summary>
        /// <param name="WritingParameters">Написанные параметры</param>
        /// <returns>Совпадает правилу или нет</returns>
        protected abstract bool AbsolutlyRequiredParameters(string[] WritingParameters);

        /// <summary>
        /// Регулярное выражение имени команды
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        [GeneratedRegex(@"[^*]+")]
        internal sealed static partial Regex RegexNameCommand();

        /// <summary>
        /// Регулярное выражение сортировки имени и параметров команды
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        [GeneratedRegex(@"\*.*")]
        internal sealed static partial Regex RegexParameterCommand();

        /// <summary>
        /// Регулярное выражение сортировки параметров от специальных символов
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        [GeneratedRegex(@"([^%,]+|%,|%%)+")]
        internal sealed static partial Regex RegexSortParamCommand();
    }
}
