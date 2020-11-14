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
        private readonly IUserInfoProvider userInfoProvider;

        public DefaultWelcomeCommand(
            ILicenseQuery licenseQuery,
            ILicenseClient licenseClient,
            IDeviceIdProvider deviceIdProvider,
            IUserInfoProvider userInfoProvider
        )
        {
            this.licenseQuery = licenseQuery;
            this.licenseClient = licenseClient;
            this.deviceIdProvider = deviceIdProvider;
            this.userInfoProvider = userInfoProvider;
        }

        public async Task<int> Execute()
        {
            WriteLine("Fetching user information...");

            var userInfo = await userInfoProvider.GetUserInfoAsync();
            if (userInfo == null)
            {
                WriteLine("ERROR: Cannot fetch user information");
                return -1;
            }

            WriteLine($"Hello {userInfo.UserName}!");
            WriteLine($"Id:    {userInfo.UserId}");
            WriteLine($"Email: {userInfo.Email}");

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
                        var trialResult = await licenseClient.LicensesRequestTrialAsync(new InRequestTrialModel(machineKey));
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
