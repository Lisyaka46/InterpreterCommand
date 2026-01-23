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

        /// <summary>
        /// Добавить новый текст исходя из входящего объекта
        /// </summary>
        /// <param name="Array">Массив зависимых объектов</param>
        /// <param name="Function">Преобразование данных объекта в строку</param>
        public void AddString<TSource>(TSource[] Array, Func<TSource, string?> Function);
    }
}
