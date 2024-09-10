using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace Interpreter.Classes
{
    /// <summary>
    /// Буфер консольных команд
    /// </summary>
    /// <remarks>
    /// Инициализировать новый буфер команд
    /// </remarks>
    /// <param name="CountBuffer">Количество сохраняемых команд в буфер</param>
    public class Buffer(int CountBuffer = 50)
    {
        /// <summary>
        /// Делегат события добавления элемента в буфер
        /// </summary>
        /// <param name="Name">Имя команды</param>
        public delegate void DelegateAddElement(string Name);

        /// <summary>
        /// Делегат события удаления элемента из буфера
        /// </summary>
        /// <param name="Index">Индекс удаляемого элемента</param>
        public delegate void DelegateIndexElement(int Index);

        /// <summary>
        /// Делегат события сортировки буфера
        /// </summary>
        public delegate void DelegateEventBuffer();

        /// <summary>
        /// Событие добавление элемента
        /// </summary>
        public event DelegateAddElement? AddElement;

        /// <summary>
        /// Событие сортировки буфера
        /// </summary>
        public event DelegateIndexElement? SortBuffer;

        /// <summary>
        /// Событие удаление элемента
        /// </summary>
        public event DelegateIndexElement? DelElement;

        /// <summary>
        /// Событие очистка буфера
        /// </summary>
        public event DelegateEventBuffer? ClearBuffer;

        /// <summary>
        /// Массив элементов буфера
        /// </summary>
        public string[] BufferElements { get; private set; } = new string[Math.Clamp(CountBuffer, 4, 80)];

        /// <summary>
        /// Количество добавленных команд
        /// </summary>
        public int Count { get; private set; } = 0;

        /// <summary>
        /// Общее количество мест в буфере
        /// </summary>
        public int Length => BufferElements.Length;

        /// <summary>
        /// Индексатор буфера элементов
        /// </summary>
        /// <param name="key">Индекс читаемого элемента</param>
        /// <returns>Прочитанный текст элемента</returns>
        /// <exception cref="IndexOutOfRangeException">Исключение выхода индекса за границы буфера</exception>
        public string this[Index key]
        {
            get
            {
                if (key.Value < Length) return BufferElements[key] ?? throw new Exception("Объект по индексу является нулевым. Данный тип не допускает пустых значений");
                else throw new IndexOutOfRangeException($"Индекс ({key}) вышел за рамки буфера ({Length})");
            }
            private set
            {
                if (key.Value < Length) BufferElements[key] = value;
                else throw new IndexOutOfRangeException($"Индекс ({key}) вышел за рамки буфера ({Length})");
            }
        }

        /// <summary>
        /// Удалить элемент буфера
        /// </summary>
        /// <param name="DeleteElement">Объект удаляемый из сетки буфера</param>
        public void Delete(string DeleteElement)
        {
            int i = Array.IndexOf(BufferElements, DeleteElement);
            if (i == -1) throw new IndexOutOfRangeException("Удаляемый элемент из буфера сохранённых команд не найден (-1)");
            else Delete(i);
        }

        /// <summary>
        /// Удалить элемент буфера
        /// </summary>
        /// <param name="index">Индекс удаляемого элемента</param>
        internal void Delete(int index)
        {
            if (Count > 0)
            {
                ReSort(index);
                Count--;
                DelElement?.Invoke(index);
                SortBuffer?.Invoke(index);
            }
        }

        /// <summary>
        /// Пересортировка исключая index
        /// </summary>
        /// <param name="index">Исключающий индекс элемента</param>
        /// <param name="AnimateAction">Анимировать сортировку или нет</param>
        private void ReSort(int index)
        {
            if (Count > 1 && index < Count - 1)
            {
                for (int i = index; i < Count - 1; i++)
                {
                    BufferElements[i] = BufferElements[i + 1];
                }
                BufferElements[Count - 1] = string.Empty;
            }
        }

        /// <summary>
        /// Удалить <b>все</b> элементы буфера
        /// </summary>
        public void DeleteAll()
        {
            if (Count > 0)
            {
                BufferElements = new string[BufferElements.Length];
                Count = 0;
                ClearBuffer?.Invoke();
            }
        }

        /// <summary>
        /// Добавить элемент в буфер <b></b>
        /// </summary>
        /// <remarks>
        /// При переполнении самый первый элемент удаляется и добавляется текущий
        /// </remarks>
        /// <param name="Command">Элемент буфера</param>
        /// <param name="Name">Имя команды</param>
        /// <param name="Parameteres">Параметры выполняемой команды</param>
        /// <param name="StringCommand">Пропись команды</param>
        /// <param name="ChildrenElements">Сетка элементов буфера</param>
        public void Add(string Command)
        {
            if (Count < BufferElements.Length)
            {
                this[Count++] = Command;
            }
            else
            {
                ReSort(0);
                this[^1] = Command;
                SortBuffer?.Invoke(0);
            }
            AddElement?.Invoke(Command);
        }
    }
}
