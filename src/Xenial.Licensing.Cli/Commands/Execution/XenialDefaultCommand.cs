using System;
using System.Collections.Generic;
using System.CommandLine;

namespace Xenial.Licensing.Cli.Commands
{
    public class XenialDefaultCommand : IXenialCommand
    {
        public virtual IEnumerable<Option> CreateOptions() => new[]
        {
            new Option(new[] { "--interactive", "-i" }, "Commands will pause and ask for arguments")
            {
                Argument = new Argument<bool>()
            },
            new Option(new[] { "--no-logo" }, "Will not output the display header")
            {
                Argument = new Argument<bool>()
            },
        };

        public bool Interactive { get; set; }
        public bool NoLogo { get; set; }

        public bool AskForKey(string message)
        {
            if (Interactive)
            {
                Console.WriteLine($"{message} Y/n");
                var key = Console.ReadKey();
                var askForKey = key.Key == ConsoleKey.Y || key.Key == ConsoleKey.Enter;
                return askForKey;
            }
            return true;
        }
    }
}
