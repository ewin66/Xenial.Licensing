using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace Xenial.Licensing.Cli.Services.Default
{
    public class DefaultApiClientConfiguration : IApiClientConfiguration
    {
        private readonly IConfiguration configuration;
        private readonly ITokenProvider tokenProvider;

        public DefaultApiClientConfiguration(IConfiguration configuration, ITokenProvider tokenProvider)
        {
            this.configuration = configuration;
            this.tokenProvider = tokenProvider;
        }
        public string BaseUrl => configuration.GetSection("Apis").GetValue<string>("Xenial.Licensing");

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            var userToken = await tokenProvider.GetUserTokenAsync();

            return userToken.AccessToken;
        }
    }
}
