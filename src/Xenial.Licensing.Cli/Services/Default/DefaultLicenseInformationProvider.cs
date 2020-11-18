using System;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services.Default
{
    public class DefaultLicenseInformationProvider : ILicenseInformationProvider
    {
        private readonly ILicenseStorage licenseStorage;
        private readonly ILicensePublicKeyStorage publicKeyStorage;

        public DefaultLicenseInformationProvider(ILicenseStorage licenseStorage, ILicensePublicKeyStorage publicKeyStorage)
        {
            this.licenseStorage = licenseStorage;
            this.publicKeyStorage = publicKeyStorage;
        }

        public async Task<DateTime> IsValidUntil()
        {
            var licString = await licenseStorage.FetchAsync();
            if (!string.IsNullOrEmpty(licString))
            {
                var lic = Standard.Licensing.License.Load(licString);
                return lic.Expiration.ToUniversalTime().Date;
            }

            return DateTime.MinValue;
        }
    }
}
