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
using DeviceId;
using System.IO;
using Xenial.Licensing.Cli.Services;

namespace Xenial.Licensing.Cli
{
    public class XenialLicensingApplication
    {
        private readonly ITokenProvider tokenProvider;

        public XenialLicensingApplication(ITokenProvider tokenProvider)
            => this.tokenProvider = tokenProvider;


        public async Task<int> RunAsync(string[] args)
        {
            try
            {
                var userToken = await tokenProvider.GetUserTokenAsync();
                WriteLine();
                WriteLine(userToken.AccessToken);
                WriteLine(userToken.RefreshToken);
                WriteLine(userToken.IdToken);

                ReadLine();
            }
            catch (Exception ex)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine(ex.Message);
                ResetColor();
                return 1;
            }

            return 0;
        }
    }
}
