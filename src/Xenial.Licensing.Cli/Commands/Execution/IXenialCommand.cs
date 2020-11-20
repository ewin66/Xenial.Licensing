using System.Collections.Generic;
using System.CommandLine;

namespace Xenial.Licensing.Cli.Commands
{
    public interface IXenialCommand
    {
        IEnumerable<Option> CreateOptions();
    }
}
