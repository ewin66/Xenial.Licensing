using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using IdentityModel.Client;

using Microsoft.Extensions.Configuration;

namespace Xenial.Licensing.Cli.Services.Default
{
    public class DefaultConfigurationProvider
    {
        private readonly IConfiguration configuration;

        public DefaultConfigurationProvider(IConfiguration configuration)
            => this.configuration = configuration;

        private IConfigurationSection AuthSection => configuration.GetSection("Authentication:Xenial");
        public string Authority => AuthSection.GetValue<string>("Authority");
        public string ClientId => AuthSection.GetValue<string>("ClientId");
        public string Scope => string.Join(" ",
            AuthSection.GetSection("Scope")
                .AsEnumerable()
                .Where(s => !string.IsNullOrEmpty(s.Value))
                .Select(s => s.Value)
                .ToArray()
        );
    }

    public class DefaultDiscoveryProvider
    {
        private readonly DefaultConfigurationProvider configurationProvider;
        private readonly HttpClient httpClient;

        public DefaultDiscoveryProvider(DefaultConfigurationProvider configurationProvider, HttpClient httpClient)
        {
            this.configurationProvider = configurationProvider;
            this.httpClient = httpClient;
        }

        public async Task<DiscoveryDocumentResponse> FetchDiscoveryDocument()
        {
            var disco = await httpClient.GetDiscoveryDocumentAsync(configurationProvider.Authority);
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }
            return disco;
        }
    }
}
