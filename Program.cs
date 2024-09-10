using Interpreter.Commands;
using System.Diagnostics;

namespace Interpreter
{
    internal class Program
    {
        /// <summary>
        /// Массив консольных команд
        /// </summary>
        static readonly List<ConsoleCommand> DataConsoleCommand =
        [
            new ConsoleCommand("reboot", "Перезагружает программу", (Main, param) =>
            {
                Process.Start(Process.GetCurrentProcess().ProcessName, Environment.GetCommandLineArgs());
                Process.GetCurrentProcess().Kill();
                return Task.FromResult(CommandStateResult.Completed(Main.Name));
            }),

            new ConsoleCommand("close", "Закрывает программу", (Main, param) =>
            {
                Process.GetCurrentProcess().Kill();
                
                return Task.FromResult(CommandStateResult.Completed(Main.Name));
            }),

            new ConsoleCommand("buffer", "Показывает содержимое буфера", (Main, param) =>
            {
                if (BufferCommand != null) Console.WriteLine("[" + string.Join(',', BufferCommand.BufferElements) + "]");
                else Console.WriteLine("[null]");
                return Task.FromResult(CommandStateResult.Completed(Main.Name));
            }),

            new ConsoleCommand("set-buffer", [new Parameter("Count", typeof(Int32))],"Показывает содержимое буфера", (Main, param) =>
            {
                if (param.Length > 0) {
                BufferCommand = new(Convert.ToInt32(param[0]));
                }
                return Task.FromResult(CommandStateResult.Completed(Main.Name));
            }),
        ];

        /// <summary>
        /// Буфер команд
        /// </summary>
        static Classes.Buffer? BufferCommand;

        static void Main()
        {
            CommandStateResult Result;
            while (true)
            {
                Console.Write("> ");
                Result = ConsoleCommand.ReadAndExecuteCommand(BufferCommand, [.. DataConsoleCommand], Console.ReadLine() ?? string.Empty);
                Console.WriteLine($"\"{Result.NameCommand}\" | State: {Result.State} | Message: \"{Result.Massage}\"");
            }
        }
    }
}
