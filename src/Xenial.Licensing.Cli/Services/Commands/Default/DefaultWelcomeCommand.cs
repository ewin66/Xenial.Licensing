using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xenial.Licensing.Cli.Services.Queries;

using static System.Console;

namespace Xenial.Licensing.Cli.Services.Commands.Default
{
    public class DefaultWelcomeCommand
    {
        private readonly ILicenseQuery licenseQuery;
        private readonly ITokenProvider tokenProvider;

        public DefaultWelcomeCommand(ILicenseQuery licenseQuery, ITokenProvider tokenProvider)
        {
            this.licenseQuery = licenseQuery;
            this.tokenProvider = tokenProvider;
        }

        public async Task<int> Execute()
        {
            WriteLine("Fetching active licenses...");

            var result = await licenseQuery.HasActiveLicense();

            WriteLine($"Has active licenses...? {result}");

            return 0;
        }
    }
}
