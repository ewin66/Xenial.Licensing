using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using IdentityModel.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Xenial.Licensing.Cli.Services.Default
{
    public class DefaultUserInfoProvider : IUserInfoProvider
    {
        private readonly HttpClient httpClient;
        private readonly ITokenProvider tokenProvider;
        private readonly IConfiguration configuration;
        private readonly ILogger<DefaultUserInfoProvider> logger;

        public DefaultUserInfoProvider(
            HttpClient httpClient,
            ITokenProvider tokenProvider,
            IConfiguration configuration,
            ILogger<DefaultUserInfoProvider> logger
        )
        {
            this.httpClient = httpClient;
            this.tokenProvider = tokenProvider;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<UserInfo> GetUserInfoAsync()
        {
            var token = await tokenProvider.GetUserTokenAsync();
            httpClient.SetBearerToken(token.AccessToken);
            var identityUrl = configuration.GetSection("Authentication:Xenial").GetValue<string>("Authority");
            var discoResult = await httpClient.GetDiscoveryDocumentAsync(identityUrl);
            if (discoResult.IsError)
            {
                return null;
            }
            var userInfoResult = await httpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = discoResult.UserInfoEndpoint,
                Token = token.AccessToken,
            });

            if (userInfoResult.IsError)
            {
                return null;
            }
            var userId = userInfoResult.Claims.FirstOrDefault(m => m.Type == "sub");
            var userName = userInfoResult.Claims.FirstOrDefault(m => m.Type == "preferred_username");
            var email = userInfoResult.Claims.FirstOrDefault(m => m.Type == "email");
            if (userId == null)
            {
                logger.LogError("Cannot fetch sub {UserInfoResult}", userInfoResult);
                return null;
            }
            if (userName == null)
            {
                logger.LogError("Cannot fetch preferred_username {UserInfoResult}", userInfoResult);
                return null;
            }
            if (email == null)
            {
                logger.LogWarning("Cannot fetch email {UserInfoResult}", userInfoResult);
            }
            return new UserInfo(userId.Value, userName.Value, email?.Value);
        }
    }
}
