using Interpreter.Classes;
using Interpreter.Commands;
using InterpreterCommand.Classes;
using InterpreterCommand.Interfaices;
using System.Diagnostics;

namespace Interpreter
{
    internal class Program
    {
        /// <summary>
        /// Массив консольных команд
        /// </summary>
        private static COMInterpreter<ICommandViewer> Interpreter = new(
            [
            new ConsoleCommand<ICommandViewer>("reboot", "Перезагружает программу", (Main, param, CV) =>
            {
                Process.Start(Process.GetCurrentProcess().ProcessName, Environment.GetCommandLineArgs());
                Process.GetCurrentProcess().Kill();
                return Task.FromResult(CommandStateResult.Completed(Main.Name));
            }),

            new ConsoleCommand<ICommandViewer>("close", "Закрывает программу", (Main, param, CV) =>
            {
                Process.GetCurrentProcess().Kill();

                return Task.FromResult(CommandStateResult.Completed(Main.Name));
            }),

            new ConsoleCommand<ICommandViewer>("buffer", "Показывает содержимое буфера", (Main, param, CV) =>
            {
                if (BufferCommand != null) Console.WriteLine("[" + string.Join(',', BufferCommand.BufferElements) + "]");
                else Console.WriteLine("[null]");
                return Task.FromResult(CommandStateResult.Completed(Main.Name));
            }),

            new ConsoleCommand<ICommandViewer>("set-buffer", [new Parameter("Count", typeof(int))],"Показывает содержимое буфера", (Main, param, CV) =>
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
                Result = Interpreter.ReadAndExecuteCommand(BufferCommand, Command, null).Result;
                Console.WriteLine($"\"{Result.NameCommand}\" | State: {Result.State} | Message: \"{Result.Message}\"");
            }
        }
    }
}
