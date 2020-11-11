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

            var clientId = "Xenial.Licensing.Cli";
            var scope = "openid profile xenial role offline_access";
            var result = await httpClient.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = disco.DeviceAuthorizationEndpoint,
                ClientId = clientId,
                Scope = scope
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

            var fetchToken = true;
            var accessToken = string.Empty;
            var identityToken = string.Empty;
            var refreshToken = string.Empty;
            var interval = (result.Interval == 0 ? 5 : result.Interval) * 1000;

            while (fetchToken)
            {
                WriteLine("Fetching token....");
                var tokenResponse = await httpClient.RequestDeviceTokenAsync(new DeviceTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = clientId,
                    DeviceCode = result.DeviceCode,
                });
                if (tokenResponse.IsError)
                {
                    if (tokenResponse.Error == "authorization_pending" || tokenResponse.Error == "slow_down")
                    {
                        WriteLine($"{tokenResponse.Error}...waiting.");
                        await Task.Delay(interval);
                    }
                    else
                    {
                        throw new Exception(tokenResponse.Error);
                    }
                }
                else
                {
                    accessToken = tokenResponse.AccessToken;
                    identityToken = tokenResponse.IdentityToken;
                    refreshToken = tokenResponse.RefreshToken;
                    fetchToken = false;
                }
            }

            WriteLine($"Fetched accessToken: {accessToken}");
            WriteLine();
            WriteLine($"Fetched identityToken: {identityToken}");
            WriteLine();
            WriteLine($"Fetched refreshToken: {refreshToken}");

            var userInfo = await httpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = accessToken
            });

            WriteLine();
            WriteLine(userInfo.Raw);

            ReadLine();

            return 0;
        }
    }
}
