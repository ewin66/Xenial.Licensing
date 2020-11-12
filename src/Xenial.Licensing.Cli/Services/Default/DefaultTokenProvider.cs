using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using IdentityModel.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using static System.Console;

namespace Xenial.Licensing.Cli.Services.Default
{
    public class DefaultTokenProvider : ITokenProvider
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;
        private readonly IUserProfileProvider userProfileProvider;
        private readonly ILogger<DefaultTokenProvider> logger;

        public DefaultTokenProvider(IConfiguration configuration, HttpClient httpClient, IUserProfileProvider userProfileProvider, ILogger<DefaultTokenProvider> logger)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
            this.userProfileProvider = userProfileProvider;
            this.logger = logger;
        }

        public async Task<UserToken> GetUserTokenAsync()
        {
            var section = configuration.GetSection("Authentication:Xenial");
            var authority = section.GetValue<string>("Authority");
            var clientId = section.GetValue<string>("ClientId");

            var disco = await httpClient.GetDiscoveryDocumentAsync(authority);
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }

            var tokenFile = Path.Combine(await userProfileProvider.GetUserProfileDirectoryAsync(), "tokens.json");

            if (File.Exists(tokenFile))
            {
                try
                {
                    var userTokens = await ReadCachedTokenAsync(tokenFile);
                    var refreshedToken = await RefreshTokensAsync(userTokens, clientId, disco);
                    if (refreshedToken != null)
                    {
                        refreshedToken = await CacheTokenAsync(tokenFile, refreshedToken);
                        if (refreshedToken != null)
                        {
                            return refreshedToken;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error deserialize UserToken");
                    try
                    {
                        File.Delete(tokenFile);
                    }
                    catch (Exception fileDeleteException)
                    {
                        logger.LogError(fileDeleteException, "Error deleting UserToken file");
                    }
                }
            }

            var scopes = section.GetSection("Scope").AsEnumerable().Where(s => !string.IsNullOrEmpty(s.Value)).Select(s => s.Value).ToArray();


            var result = await httpClient.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = disco.DeviceAuthorizationEndpoint,
                ClientId = clientId,
                Scope = string.Join(" ", scopes)
            });

            if (result.IsError)
            {
                throw new Exception(result.Error);
            }

            WriteLine();
            WriteLine();
            WriteLine(Consts.Header);
            WriteLine();
            WriteLine();
            WriteLine($"Visit: {result.VerificationUri}");
            WriteLine();
            WriteLine("And enter this code");
            WriteLine("-------------------");
            WriteLine($"-    {result.UserCode}    -");
            WriteLine("-------------------");

            var fetchToken = true;
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
                    var token = new UserToken(tokenResponse.AccessToken, tokenResponse.RefreshToken, tokenResponse.IdentityToken);
                    await CacheTokenAsync(tokenFile, token);
                    return token;
                }
            }
            return null;
        }

        private static async Task<UserToken> ReadCachedTokenAsync(string tokenFile)
        {
            using var fileStream = File.OpenRead(tokenFile);
            return await JsonSerializer.DeserializeAsync<UserToken>(fileStream);
        }

        private async Task<UserToken> CacheTokenAsync(string tokenFile, UserToken refreshedToken)
        {
            try
            {
                if (refreshedToken == null)
                {
                    return null;
                }

                await File.WriteAllTextAsync(tokenFile, JsonSerializer.Serialize(refreshedToken, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error caching token");
                return null;
            }
            return refreshedToken;
        }

        private async Task<UserToken> RefreshTokensAsync(UserToken userTokens, string clientId, DiscoveryDocumentResponse disco)
        {
            try
            {
                var refreshResult = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = clientId,
                    RefreshToken = userTokens.RefreshToken
                });
                if (refreshResult.IsError)
                {
                    throw new Exception(refreshResult.Error);
                }
                return new UserToken(refreshResult.AccessToken, refreshResult.RefreshToken, refreshResult.IdentityToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error refreshing token");
            }
            return null;
        }
    }
}
