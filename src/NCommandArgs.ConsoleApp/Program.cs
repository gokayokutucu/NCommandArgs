using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCommandArgs.ConsoleApp
{
    class Program
    {
        private static bool _showHelp;
        public static Guid UniqueId { get; set; }
        static async Task Main(string[] args)
        {
            if (args.Length < 1)
                throw new ArgumentOutOfRangeException("args", "You must use at least one argument");

            //if (args.Length > 3)
            //    throw new ArgumentOutOfRangeException("args", "Too much arguments");

            var commandSet = new CommandSet(args)
            {
                {
                    "-?|--help|-h", "Show help information",
                    () => _showHelp = true
                },
                {
                    "init|-i", "Set an unique identifier",
                    (Guid _) =>
                    {
                        UniqueId = _;
                        Console.WriteLine(UniqueId);
                    }
                },
                {
                    "set|-s", "Set a key value pair",
                    (IEnumerable<string> _) =>
                    {
                        _.ToList().ForEach(Console.WriteLine);
                    }
                },
                {
                    "remove|-r", "Remove a key value pair",
                    (string _) =>
                    {
                        Console.WriteLine(_);
                    }
                },
                {
                    "clear|-clr", "Clear the screen",
                    Console.Clear
                }
            };

            commandSet.Execute();


            if (_showHelp)
                ShowHelp(commandSet);

            Console.ReadLine();
        }

        static void ShowHelp(CommandSet commandSet)
        {
            Console.WriteLine("NCommandArgs 1.0.0.0");
            Console.WriteLine();
            Console.WriteLine("Usage: nCommandArgs [command]");
            Console.WriteLine();
            Console.WriteLine("Commands:");

            commandSet.WriteCommandDescriptions(Console.Out);
        }
    }
}
