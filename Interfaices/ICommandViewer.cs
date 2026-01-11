using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InterpreterCommand.Interfaices
{
    /// <summary>
    /// Интерфейс объекта визуализирования команды
    /// </summary>
    public interface ICommandViewer
    {
        /// <summary>
        /// Вывести текст через визуализационный объект
        /// </summary>
        /// <param name="Source">Добавляемый текст</param>
        public void AddString(string Source);
    }
}
