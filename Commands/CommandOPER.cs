using Interpreter.Classes;
using Interpreter.Interfaces;
using InterpreterCommand.Interfaices;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterCommand.Commands
{
    /// <summary>
    /// Инициализировать объект консольной команды с параметрами
    /// </summary>
    /// <param name="name">Имя</param>
    /// <param name="parameters">Параметры команды</param>
    /// <param name="description">Описание команды</param>
    /// <param name="execute">Действие выполнения</param>
    public abstract class CommandOPER<TViewer>
        : ICommandOPER<TViewer> where TViewer : ICommandViewer
    {
        /// <summary>
        /// Имя команды
        /// </summary>
        [AllowNull()]
        public string Name { get; protected set; }

        /// <summary>
        /// Описание консольной команды
        /// </summary>
        [AllowNull()]
        public string Description { get; protected set; }

        /// <summary>
        /// Параметры команды
        /// </summary>
        [AllowNull()]
        public Parameter[] Parameters { get; protected set; }

        /// <summary>
        /// Объект токена управления асинхронным процессом
        /// </summary>
        private CancellationTokenSource? SourceTokenWhileProcess { get; set; } = null;

        /// <summary>
        /// Состояние активного токена управления асинхронным процессом
        /// </summary>
        public bool IsAsyncTokenWhileProcessEnabled => (SourceTokenWhileProcess?.Token.CanBeCanceled) ?? false;

        /// <summary>
        /// Активное состояние исполнения команды (true, если исполняется; иначе false)
        /// </summary>
        private bool IsExecutableCommand = false;

        /// <summary>
        /// Делегат события выполнения команды
        /// </summary>
        /// <param name="MainCommand">Выполняющаяся команда</param>
        /// <param name="ParametersValue">Параметры команды</param>
        /// <param name="CommandViewer">Объект визуализирующий команду</param>
        /// <returns>Итог выполнения команды</returns>
        public delegate Task<CommandStateResult> ExecuteCom(CommandOPER<TViewer> MainCommand, object[] ParametersValue, TViewer? CommandViewer);

        /// <summary>
        /// Действие которое выполняет команда
        /// </summary>
        [AllowNull()]
        internal ExecuteCom Execute;

        ///// <summary>
        ///// Инициализировать объект консольной команды без параметров
        ///// </summary>
        ///// <param name="Name">Имя</param>
        ///// <param name="Description">Описание команды</param>
        ///// <param name="Execute">Действие выполнения</param>
        //public ConsoleCommand(string Name, string Description, ExecuteCom Execute)
        //{
        //    this.Name = Name;
        //    this.Description = Description;
        //    this.Execute = Execute;
        //}

        /// <summary>
        /// Узнать написаны ли обязательные параметры команды
        /// </summary>
        /// <param name="WritingParameters">Написанные параметры</param>
        /// <returns>Совпадает правилу или нет</returns>
        public bool AbsolutlyRequiredParameters(string[] WritingParameters)
        {
            if (Parameters == null) return true;
            else return WritingParameters.Length >= Parameters.Count((i) => i.Absolutly == true);
        }

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        /// <param name="Param">Параметры команды</param>
        /// <param name="CommandViewer">Объект визуализирующий команду</param>
        public async Task<CommandStateResult> ExecuteCommand(string[] Param, TViewer? CommandViewer)
        {
            if (IsExecutableCommand)
                throw new InvalidOperationException("Невозможно рекурсивно исполнить команду.");
            else if (!AbsolutlyRequiredParameters(Param)) return CommandStateResult.FaledParameteres(Name);
            object[] MainParameters = [];
            if (Parameters != null)
            {
                MainParameters = new object[Parameters.Length];
                for (int i = 0; i < Parameters.Length; i++)
                {
                    try
                    {
                        if (Parameters[i].TypeP == typeof(int))
                        {
                            try
                            {
                                MainParameters[i] = Convert.ToInt32(Param[i]);
                            }
                            catch (FormatException) { return CommandStateResult.FaledTypeParameteres(Name, i + 1); }
                        }
                        else if (Parameters[i].TypeP == typeof(bool))
                        {
                            if (Param[i].ToLower().Equals("true") || Param[i].Equals("1")) MainParameters[i] = true;
                            else if (Param[i].ToLower().Equals("false") || Param[i].Equals("0")) MainParameters[i] = false;
                            else return CommandStateResult.FaledTypeParameteres(Name, i + 1);
                        }
                        else MainParameters[i] = Param[i];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        MainParameters[i] = Parameters[i].DefValue ?? string.Empty;
                    }
                }
            }
            IsExecutableCommand = true;
            CommandStateResult Result = await Execute.Invoke(this, MainParameters, CommandViewer);
            CloseAsyncOperation();
            IsExecutableCommand = false;
            return Result;
        }

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        /// <param name="CommandViewer">Объект визуализирующий команду</param>
        public async Task<CommandStateResult> ExecuteCommand(TViewer? CommandViewer)
        {
            if (IsExecutableCommand)
                throw new InvalidOperationException("Невозможно рекурсивно исполнить команду.");
            else if (Parameters == null)
            {
                IsExecutableCommand = true;
                CommandStateResult Result = await Execute.Invoke(this, [], CommandViewer);
                CloseAsyncOperation();
                IsExecutableCommand = false;
                return Result;
            }
            return CommandStateResult.FaledParameteres(Name);
        }

        /// <summary>
        /// Закрыть асинхронную операцию исполнения команды
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void CloseAsyncOperation()
        {
            if (SourceTokenWhileProcess != null)
            {
                if (!SourceTokenWhileProcess.IsCancellationRequested)
                    throw new Exception("Исполнение асинхронного процесса команды не закончено.");
                SourceTokenWhileProcess.Dispose();
                GC.Collect();
                SourceTokenWhileProcess = null;
            }
        }

        /// <summary>
        /// Асинхронно ожидать исполнения операции
        /// </summary>
        /// <param name="Source">Операция исполнения</param>
        /// <param name="WhileProcess">Непрерывный повторяющийся процесс операции, до закрытия прочесса токеном</param>
        /// <exception cref="Exception"></exception>
        public async Task WaitAsyncToken(Action Source, bool WhileProcess = false)
        {
            if (!IsExecutableCommand)
                throw new Exception("Невозможно зарегестрировать асинхронный токен команды вне исполнения операции текущей команды.");
            SourceTokenWhileProcess = new();
            if (!WhileProcess)
            {
                await Task.Run(Source).WaitAsync(SourceTokenWhileProcess.Token);
                if (SourceTokenWhileProcess != null)
                {
                    SourceTokenWhileProcess.Dispose();
                    GC.Collect();
                    SourceTokenWhileProcess = null;
                }
            }
            else
            {
                try
                {
                    await Task.Run(() =>
                    {
                        while (!SourceTokenWhileProcess.IsCancellationRequested)
                        {
                            Source.Invoke();
                        }
                    }).WaitAsync(SourceTokenWhileProcess.Token);
                } catch { return; }
            }
        }

        /// <summary>
        /// Закрыть исполняемую асинхронную операцию
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void CloseAsyncToken()
        {
            if (!IsExecutableCommand || SourceTokenWhileProcess == null)
                throw new Exception("Невозможно закрыть асинхронный токен команды вне исполнения операции текущей команды.");
            SourceTokenWhileProcess.Cancel(false);
        }
    }
}
