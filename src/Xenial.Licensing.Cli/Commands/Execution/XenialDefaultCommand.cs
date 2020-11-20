using System.Collections.Generic;
using System.CommandLine;

namespace Xenial.Licensing.Cli.Commands
{
    public class XenialDefaultCommand : IXenialCommand
    {
        public virtual IEnumerable<Option> CreateOptions() => new[]
        {
            new Option(new[] { "interactive", "i" }, "Commands will pause and ask for arguments")
            {
                Argument = new Argument<bool>()
            }
        };

        public bool Interactive { get; set; }
    }
}
