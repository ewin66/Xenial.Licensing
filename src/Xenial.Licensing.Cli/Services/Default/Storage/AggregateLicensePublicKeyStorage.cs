using System.Collections.Generic;
using System.Threading.Tasks;

using Xenial.Licensing.Cli.XenialLicenseApi;

namespace Xenial.Licensing.Cli.Services.Default.Storage
{
    public class AggregateLicensePublicKeyStorage : ILicensePublicKeyStorage
    {
        private readonly IList<ILicensePublicKeyStorage> storages;

        public AggregateLicensePublicKeyStorage(ILicenseClient client, IUserProfileProvider userProfileProvider)
            => storages = new ILicensePublicKeyStorage[]
            {
                new LicensePublicKeyEnvironmentStorage(),
                new LicensePublicKeyFileStorage(userProfileProvider),
                new LicensePublicKeyApiStorage(client)
            };

        public async Task<string> FetchAsync(string keyName)
        {
            foreach (var storage in storages)
            {
                var publicKey = await storage.FetchAsync(keyName);
                await storage.StoreAsync(keyName, publicKey);
                if (!string.IsNullOrEmpty(publicKey))
                {
                    return publicKey;
                }
            }

            return null;
        }

        public async Task StoreAsync(string keyName, string publicKey)
        {
            foreach (var storage in storages)
            {
                await storage.StoreAsync(keyName, publicKey);
            }
        }
    }
}
