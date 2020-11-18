using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Xenial.Licensing.Cli.XenialLicenseApi;

namespace Xenial.Licensing.Cli.Services.Default.Storage
{
    public class LicensePublicKeyApiStorage : ILicensePublicKeyStorage
    {
        private readonly ILicenseClient client;

        public LicensePublicKeyApiStorage(ILicenseClient client)
            => this.client = client;

        public async Task<string> FetchAsync(string keyName)
        {
            var publicKey = await client.PublickeyNameAsync(keyName);

            if (publicKey != null)
            {
                return publicKey.PublicKey;
            }

            return null;
        }

        public Task StoreAsync(string name, string publicKey)
            => Task.CompletedTask;
    }
}
