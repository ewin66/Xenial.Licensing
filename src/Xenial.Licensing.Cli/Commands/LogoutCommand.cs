using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using IdentityModel.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Xenial.Licensing.Cli.Services;
using Xenial.Licensing.Cli.Services.Default;
using Xenial.Licensing.Cli.Utils;

namespace Xenial.Licensing.Cli.Commands
{
    public class LogoutCommand : XenialDefaultCommand
    {
        public override IEnumerable<Option> CreateOptions()
            => base.CreateOptions()
            .Concat(new Option[]
            {
                new Option(new[] { "--no-cache" }, "Logout will not use cached arguments")
                {
                    Argument = new Argument<bool>()
                },
                new Option(new[] { "--no-store" }, "Logout will not store credentials")
                {
                    Argument = new Argument<bool>()
                }
            });

        public bool NoCache { get; set; }
        public bool NoStore { get; set; }
    }

    [XenialCommandHandler("logout")]
    public class LogoutCommandHandler : XenialCommandHandler<LoginCommand>
    {
        private readonly HttpClient httpClient;
        private readonly IUserProfileProvider userProfileProvider;
        private readonly ILogger<LoginCommandHandler> logger;
        private readonly ITokenStorage tokenStorage;
        private readonly DefaultConfigurationProvider configurationProvider;
        private readonly DefaultDiscoveryProvider discoveryProvider;
        private readonly ILicensePublicKeyStorage licensePublicKeyStorage;
        private readonly ILicenseStorage licenseStorage;

        public LogoutCommandHandler(
            HttpClient httpClient,
            IUserProfileProvider userProfileProvider,
            ILogger<LoginCommandHandler> logger,
            ITokenStorage tokenStorage,
            DefaultConfigurationProvider configurationProvider,
            DefaultDiscoveryProvider discoveryProvider,
            ILicensePublicKeyStorage licensePublicKeyStorage,
            ILicenseStorage licenseStorage
        )
        {
            this.httpClient = httpClient;
            this.userProfileProvider = userProfileProvider;
            this.logger = logger;
            this.tokenStorage = tokenStorage;
            this.configurationProvider = configurationProvider;
            this.discoveryProvider = discoveryProvider;
            this.licensePublicKeyStorage = licensePublicKeyStorage;
            this.licenseStorage = licenseStorage;
        }

        protected async override Task<int> ExecuteCommand(LoginCommand arguments)
        {
            Console.WriteLine("Logging out...");
            logger.LogInformation("Logging out... with {Arguments}", arguments);

            var userToken = await tokenStorage.LoadUserTokenAsync(); //Fetch before deleting tokens
            logger.LogInformation("Loaded {UserToken}", userToken);

            Console.WriteLine($"Deleting tokens...");
            await tokenStorage.DeleteAsync();
            Console.WriteLine($"Deleted tokens.");

            Console.WriteLine($"Deleting public keys...");
            await licensePublicKeyStorage.DestroyAsync();
            Console.WriteLine($"Deleted public keys.");

            Console.WriteLine($"Deleting stored licenses...");
            await licenseStorage.DeleteAsync();
            Console.WriteLine($"Deleted stored licenses.");

            if (userToken is null)
            {
                Console.WriteLine("User was not logged in. Can not revoke tokens...");
                Console.WriteLine("Logout was successful.");
                return 0;
            }

            var disco = await discoveryProvider.FetchDiscoveryDocument();
            var revokeRefreshTokenResult = await RevokeRefreshToken(userToken, disco);
            if (revokeRefreshTokenResult > 0)
            {
                return revokeRefreshTokenResult;
            }
            var revokeAccessTokenResult = await RevokeAccessToken(userToken, disco);
            if (revokeAccessTokenResult > 0)
            {
                return revokeAccessTokenResult;
            }

            Console.WriteLine("Logout was successful.");

            return 0;
        }

        private Task<int> RevokeRefreshToken(UserToken userToken, DiscoveryDocumentResponse disco)
            => RevokeToken(userToken.RefreshToken, "refresh_token", disco);

        private Task<int> RevokeAccessToken(UserToken userToken, DiscoveryDocumentResponse disco)
            => RevokeToken(userToken.AccessToken, "access_token", disco);

        private async Task<int> RevokeToken(string token, string tokenTypeHint, DiscoveryDocumentResponse disco)
        {
            Console.WriteLine($"Revoking {tokenTypeHint}...");
            var request = new TokenRevocationRequest
            {
                Address = disco.DeviceAuthorizationEndpoint,
                ClientId = configurationProvider.ClientId,
                Token = token,
                TokenTypeHint = tokenTypeHint
            };

            logger.LogInformation("TokenRevocationRequest {Request}", request);

            var result = await httpClient.RevokeTokenAsync(request);

            if (result.IsError)
            {
                Console.WriteLine(result.Error);
                var ex = new Exception(result.Error);
                logger.LogError(ex, "RevokeTokenAsync {Request} {Response}", request, result);
                throw ex;
            }
            else
            {
                logger.LogInformation("RevokeTokenAsync {Request} {Response}", request, result);
            }
            Console.WriteLine($"Revoked {tokenTypeHint}...");
            return 0;
        }
    }
}
