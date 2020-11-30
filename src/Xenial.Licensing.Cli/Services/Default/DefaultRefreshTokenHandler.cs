using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IdentityModel.Client;

using Microsoft.Extensions.Logging;

namespace Xenial.Licensing.Cli.Services.Default
{
    public class DefaultRefreshTokenHandler
    {
        private readonly DefaultConfigurationProvider configurationProvider;
        private readonly DefaultDiscoveryProvider discoveryProvider;
        private readonly HttpClient httpClient;
        private readonly ILogger<DefaultRefreshTokenHandler> logger;

        public DefaultRefreshTokenHandler(
            DefaultConfigurationProvider configurationProvider,
            DefaultDiscoveryProvider discoveryProvider,
            HttpClient httpClient,
            ILogger<DefaultRefreshTokenHandler> logger
        )
        {
            this.configurationProvider = configurationProvider;
            this.discoveryProvider = discoveryProvider;
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<UserToken> RefreshTokenAsync(UserToken userToken)
        {
            try
            {
                if (userToken == null)
                {
                    return null;
                }

                if (DateTime.UtcNow.AddMinutes(10) >= userToken.ExpiresAt)
                {
                    userToken = await RefreshToken(userToken);
                    return userToken;
                }

                return userToken;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error refreshing token");
            }
            return null;
        }

        private async Task<UserToken> RefreshToken(UserToken userToken)
        {
            var disco = await discoveryProvider.FetchDiscoveryDocument();

            var refreshResult = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = configurationProvider.ClientId,
                RefreshToken = userToken.RefreshToken
            });

            if (refreshResult.IsError)
            {
                logger.LogError(new Exception(refreshResult.Error), "Error refreshing token");
                return null;
            }

            return new UserToken(
                refreshResult.AccessToken,
                refreshResult.RefreshToken,
                refreshResult.IdentityToken,
                DateTime.UtcNow.AddSeconds(refreshResult.ExpiresIn)
            );
        }
    }
}
