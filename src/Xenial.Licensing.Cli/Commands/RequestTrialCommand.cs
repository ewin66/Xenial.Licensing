using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Microsoft.Extensions.Logging;

using Xenial.Licensing.Cli.Services;
using Xenial.Licensing.Cli.Services.Queries;
using Xenial.Licensing.Cli.XenialLicenseApi;

using static System.Console;

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
        private readonly ILicenseClient licenseClient;
        private readonly IDeviceIdProvider deviceIdProvider;
        private readonly IUserInfoProvider userInfoProvider;
        private readonly ILicenseStorage licenseStorage;
        private readonly ILicensePublicKeyStorage publicKeyStorage;
        private readonly ILicenseValidator licenseValidator;
        private readonly ILicenseInformationProvider licenseInformationProvider;
        private readonly ILogger<RequestTrialCommandHandler> logger;

        public RequestTrialCommandHandler(
            ILicenseClient licenseClient,
            IDeviceIdProvider deviceIdProvider,
            IUserInfoProvider userInfoProvider,
            ILicenseStorage licenseStorage,
            ILicensePublicKeyStorage publicKeyStorage,
            ILicenseValidator licenseValidator,
            ILicenseInformationProvider licenseInformationProvider,
            ILogger<RequestTrialCommandHandler> logger
        )
        {
            this.licenseClient = licenseClient;
            this.deviceIdProvider = deviceIdProvider;
            this.userInfoProvider = userInfoProvider;
            this.licenseStorage = licenseStorage;
            this.publicKeyStorage = publicKeyStorage;
            this.licenseValidator = licenseValidator;
            this.licenseInformationProvider = licenseInformationProvider;
            this.logger = logger;
        }

        protected override async Task<int> ExecuteCommand(RequestTrialCommand arguments)
        {
            WriteLine("Fetching user information...");
            logger.LogInformation("Fetching user information...");

            var userInfo = await userInfoProvider.GetUserInfoAsync();
            if (userInfo == null)
            {
                WriteLine("ERROR: Cannot fetch user information");
                logger.LogError("ERROR: Cannot fetch user information");
                return -1;
            }
            else
            {
                logger.LogInformation("Logged in with {User}", userInfo);
            }

            WriteLine($"Hello {userInfo.UserName}!");
            WriteLine($"Id:    {userInfo.UserId}");
            WriteLine($"Email: {userInfo.Email}");

            WriteLine("Fetching active licenses...");
            logger.LogInformation("Fetching active licenses...");

            var storedLicense = await licenseStorage.FetchAsync();

            WriteLine($"Has stored license...? {!string.IsNullOrEmpty(storedLicense)}");

            if (string.IsNullOrEmpty(storedLicense))
            {
                var askForKey = arguments.AskForKey("You don't have a license yet. Do you want to request a trial?");

                if (askForKey)
                {
                    try
                    {
                        var machineKey = await deviceIdProvider.GetDeviceIdAsync();
                        var trialResult = await licenseClient.LicensesRequestTrialAsync(new InRequestTrialModel(machineKey));

                        if (await licenseValidator.IsValid(trialResult.License, trialResult.PublicKey))
                        {
                            WriteLine($"License is valid until {await licenseInformationProvider.IsValidUntil()}");
                        }

                        if (!arguments.NoStore)
                        {
                            await licenseStorage.StoreAsync(trialResult.License);
                            await publicKeyStorage.StoreAsync(trialResult.PublicKeyName, trialResult.PublicKey);
                        }

                        return 0;
                    }
                    catch (LicenseApiException<IDictionary<string, object>> ex) when (ex.StatusCode == 400)
                    {
                        WriteLine($"ERROR");
                        foreach (var error in ex.Result)
                        {
                            WriteLine($"{error.Key}: {error.Value}");
                        }
                    }
                    catch (LicenseApiException ex) when (ex.StatusCode == 400 || ex.StatusCode == 500)
                    {
                        WriteLine($"ERROR {ex}");
                    }
                }
            }

            if (await licenseValidator.IsValid(storedLicense))
            {
                WriteLine($"License is valid until {await licenseInformationProvider.IsValidUntil()}");
            }

            return 0;
        }
    }
}
