using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Commands
{
    public class RequestTrialCommand : XenialDefaultCommand
    {
        public override IEnumerable<Option> CreateOptions()
            => base.CreateOptions()
            .Concat(new Option[]
            {
                new Option(new[] { "--no-cache" }, "Login will not use cached arguments")
                {
                    Argument = new Argument<bool>()
                },
                new Option(new[] { "--no-store" }, "Login will not store credentials")
                {
                    Argument = new Argument<bool>()
                }
            });

        public bool NoCache { get; set; }
        public bool NoStore { get; set; }
    }

    [XenialCommandHandler("request-trial")]
    public class RequestTrialCommandHandler : XenialCommandHandler<RequestTrialCommand>
    {
        protected override Task<int> ExecuteCommand(RequestTrialCommand arguments) => throw new NotImplementedException();
    }
}
