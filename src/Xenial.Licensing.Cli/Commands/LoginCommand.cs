using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Commands
{
    public class LoginCommand : IXenialCommand
    {
        public IEnumerable<Option> CreateOptions() => Enumerable.Empty<Option>();
    }

    [XenialCommandHandler(typeof(LoginCommand), "login")]
    public class LoginCommandHandler : XenialCommandHandler<LoginCommand>
    {
        protected override Task<int> ExecuteCommand(LoginCommand arguments)
        {
            Console.WriteLine("Logging in...");
            return Task.FromResult(0);
        }
    }
}
