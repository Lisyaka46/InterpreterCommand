using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Interpreter.Classes
{
    /// <summary>
    /// Параметер команды
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Имя параметра команды
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Значение параметра команды
        /// </summary>
        public readonly bool Absolutly;

        /// <summary>
        /// Тип параметра команды
        /// </summary>
        public readonly Type TypeP;

        /// <summary>
        /// Значение по умолчанию для параметра
        /// </summary>
        public readonly object? DefValue = null;

        /// <summary>
        /// Создать экземпляр <b>ОБЯЗАТЕЛЬНОГО</b> параметра
        /// </summary>
        /// <param name="NameParameter">Имя параметра</param>
        /// <param name="TypeParameter">Тип параметра</param>
        public Parameter(string NameParameter, Type TypeParameter)
        {
            Name = NameParameter;
            Absolutly = true;
            TypeP = TypeParameter;
        }

        /// <summary>
        /// Создать экземпляр <b>НЕ ОБЯЗАТЕЛЬНОГО</b> параметра
        /// </summary>
        /// <param name="NameParameter">Имя параметра</param>
        /// <param name="DefaultValue">Значение по умолчанию</param>
        public Parameter(string NameParameter, Type TypeParameter, object DefaultValue)
        {
            Name = NameParameter;
            Absolutly = false;
            DefValue = DefaultValue;
            TypeP = TypeParameter;
        }
    }
}
