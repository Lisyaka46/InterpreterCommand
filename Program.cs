using AAC20.Classes.Commands;
using System.Diagnostics;
using static AAC20.Classes.Buffer;

namespace InterpreterCommand
{
    internal class Program
    {
        /// <summary>
        /// Массив консольных команд
        /// </summary>
        static readonly List<ConsoleCommand> DataConsoleCommand =
        [
            new ConsoleCommand("reboot", "Перезагружает программу", (param) =>
            {
                Process.Start(Process.GetCurrentProcess().ProcessName, Environment.GetCommandLineArgs());
                Process.GetCurrentProcess().Kill();
                return Task.FromResult(CommandStateResult.Completed);
            }),

            new ConsoleCommand("close", "Закрывает программу", (param) =>
            {
                Process.GetCurrentProcess().Kill();
                return Task.FromResult(CommandStateResult.Completed);
            }),

            new ConsoleCommand("buffer", "Показывает содержимое буфера", (param) =>
            {
                if (BufferCommand != null) Console.WriteLine("[" + string.Join(',', BufferCommand.BufferElements) + "]");
                else Console.WriteLine("[null]");
                return Task.FromResult(CommandStateResult.Completed);
            }),

            new ConsoleCommand("set-buffer", [new Parameter("Count")],"Показывает содержимое буфера", (param) =>
            {
                if (param.Length > 0) {
                BufferCommand = new(Convert.ToInt32(param[0]));
                }
                return Task.FromResult(CommandStateResult.Completed);
            }),
        ];

        /// <summary>
        /// Буфер команд
        /// </summary>
        static AAC20.Classes.Buffer? BufferCommand;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("> ");
                Console.WriteLine(ConsoleCommand.ReadAndExecuteCommand(BufferCommand, [.. DataConsoleCommand], Console.ReadLine() ?? string.Empty).Massage);
            }
        }
    }
}
