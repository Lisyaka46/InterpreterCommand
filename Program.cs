using Interpreter.Classes;
using Interpreter.Commands;
using InterpreterCommand.Classes;
using System.Diagnostics;

namespace Interpreter
{
    internal class Program
    {
        /// <summary>
        /// Массив консольных команд
        /// </summary>
        private static COMInterpreter Interpreter = new(
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

            new ConsoleCommand("set-buffer", [new Parameter("Count", typeof(int))],"Показывает содержимое буфера", (Main, param) =>
            {
                BufferCommand = new(Convert.ToInt32(param[0]));
                return Task.FromResult(CommandStateResult.Completed(Main.Name));
            }),
            ]);

        /// <summary>
        /// Буфер команд
        /// </summary>
        static Classes.Buffer? BufferCommand;

        static void Main()
        {
            CommandStateResult Result;
            string Command;
            while (true)
            {
                Console.Write("> ");
                Command = Console.ReadLine() ?? string.Empty;
                Result = Interpreter.ReadAndExecuteCommand(BufferCommand, Command);
                Console.WriteLine($"\"{Result.NameCommand}\" | State: {Result.State} | Message: \"{Result.Message}\"");
            }
        }
    }
}
