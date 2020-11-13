using System.Threading.Tasks;

using Xenial.Licensing.Cli.XenialLicenseApi;

namespace Xenial.Licensing.Cli.Services.Queries.Default
{
    public class DefaultLicenseQuery : ILicenseQuery
    {
        private readonly ILicenseClient client;

        public DefaultLicenseQuery(ILicenseClient client)
            => this.client = client;

        public async Task<bool> HasActiveLicense()
        {
            var result = await client.LicensesActiveAsync();
            if (result.Count == 0)
            {
                return false;
            }

            return result.Count > 0;
        }
    }
}
