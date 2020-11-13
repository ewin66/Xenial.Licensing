using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xenial.Licensing.Cli.Services.Queries;
using Xenial.Licensing.Cli.XenialLicenseApi;

using static System.Console;

namespace Xenial.Licensing.Cli.Services.Commands.Default
{
    public class DefaultWelcomeCommand
    {
        private readonly ILicenseQuery licenseQuery;
        private readonly ILicenseClient licenseClient;
        private readonly IDeviceIdProvider deviceIdProvider;

        public DefaultWelcomeCommand(
            ILicenseQuery licenseQuery,
            ILicenseClient licenseClient,
            IDeviceIdProvider deviceIdProvider
        )
        {
            this.licenseQuery = licenseQuery;
            this.licenseClient = licenseClient;
            this.deviceIdProvider = deviceIdProvider;
        }

        public async Task<int> Execute()
        {
            WriteLine("Fetching active licenses...");

            var result = await licenseQuery.HasActiveLicense();

            WriteLine($"Has active licenses...? {result}");
            if (!result)
            {
                WriteLine($"You don't have a license yet. Do you want to request a trial? Y/n");
                var key = ReadKey();
                if (key.Key == ConsoleKey.Y || key.Key == ConsoleKey.Enter)
                {
                    try
                    {
                        var machineKey = await deviceIdProvider.GetDeviceIdAsync();
                        var trialResult = await licenseClient.LicensesNewTrialAsync(new InRequestTrialModel(machineKey));
                    }
                    catch (LicenseApiException ex) when (ex.StatusCode == 400)
                    {
                        WriteLine($"ERROR {ex}");
                    }
                }
            }

            return 0;
        }
    }
}
