using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace Xenial.Licensing.Cli.Services.Queries
{
    public class LicenseQuery
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        public LicenseQuery(IConfiguration configuration, HttpClient httpClient)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
        }

        public async Task<bool> HasActiveLisence()
        {
            var client = new Xenial.Licensing.Cli.Models.swaggerClient("", httpClient);
            var result = await client.LicensesAsync();
            return true;
        }
    }
}
