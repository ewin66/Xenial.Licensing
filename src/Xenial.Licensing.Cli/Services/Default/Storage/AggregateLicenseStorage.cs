using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services.Default.Storage
{
    public class AggregateLicenseStorage : ILicenseStorage
    {
        private readonly IList<ILicenseStorage> storages;

        public AggregateLicenseStorage(IUserProfileProvider userProfileProvider)
            => storages = new ILicenseStorage[]
            {
                new LicenseEnvironmentStorage(),
                new LicenseFileStorage(userProfileProvider)
            };

        public async Task<string> FetchAsync()
        {
            foreach (var storage in storages)
            {
                var license = await storage.FetchAsync();
                if (!string.IsNullOrEmpty(license))
                {
                    return license;
                }
            }

            return null;
        }

        public async Task StoreAsync(string license)
        {
            foreach (var storage in storages)
            {
                await storage.StoreAsync(license);
            }
        }

        public async Task DeleteAsync()
        {
            foreach (var storage in storages)
            {
                await storage.DeleteAsync();
            }
        }
    }
}
