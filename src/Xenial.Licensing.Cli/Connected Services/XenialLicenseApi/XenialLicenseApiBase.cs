using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using IdentityModel.Client;

using Xenial.Licensing.Cli.Services;

namespace Xenial.Licensing.Cli.XenialLicenseApi
{
    public abstract class XenialLicenseApiBase
    {
        private readonly IApiClientConfiguration apiClientConfiguration;

        protected string BaseUrl => apiClientConfiguration.BaseUrl;

        public XenialLicenseApiBase(IApiClientConfiguration apiClientConfiguration)
            => this.apiClientConfiguration = apiClientConfiguration;

        protected async Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage();
            var accessToken = await apiClientConfiguration.GetAccessTokenAsync(cancellationToken);
            message.SetBearerToken(accessToken);
            return message;
        }
    }
}
