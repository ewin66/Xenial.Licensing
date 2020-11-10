using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using IdentityModel;

using static System.Console;
using System.Net.Http;
using IdentityModel.Client;

namespace Xenial.Licensing.Cli
{
    public class XenialLicensingApplication
    {
        private const string header = @"__   __          _       _ 
\ \ / /         (_)     | |
 \ V / ___ _ __  _  __ _| |
  > < / _ \ '_ \| |/ _` | |
 / . \  __/ | | | | (_| | |
/_/ \_\___|_| |_|_|\__,_|_|
                           ";

        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        public XenialLicensingApplication(IConfiguration configuration, HttpClient httpClient)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
        }

        public async Task<int> RunAsync(string[] args)
        {
            var authority = configuration.GetSection("Authentication:Xenial").GetValue<string>("Authority");
            var disco = await httpClient.GetDiscoveryDocumentAsync(authority);

            var result = await httpClient.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = disco.DeviceAuthorizationEndpoint,
                ClientId = "Xenial.Licensing.Cli",
            });

            WriteLine();
            WriteLine();
            WriteLine(header);
            WriteLine();
            WriteLine();
            WriteLine($"Visit: {result.VerificationUri}");
            WriteLine();
            WriteLine("And enter this code");
            WriteLine("-------------------");
            WriteLine($"-    {result.UserCode}    -");
            WriteLine("-------------------");

            ReadLine();

            return 0;
        }
    }
}
