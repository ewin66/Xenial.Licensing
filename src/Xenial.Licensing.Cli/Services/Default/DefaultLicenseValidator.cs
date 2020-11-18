using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Standard.Licensing.Validation;

namespace Xenial.Licensing.Cli.Services.Default
{
    public class DefaultLicenseValidator : ILicenseValidator
    {
        private readonly ILicenseStorage licenseStorage;
        private readonly ILicensePublicKeyStorage publicKeyStorage;

        public DefaultLicenseValidator(ILicenseStorage licenseStorage, ILicensePublicKeyStorage publicKeyStorage)
        {
            this.licenseStorage = licenseStorage;
            this.publicKeyStorage = publicKeyStorage;
        }

        public async Task<bool> IsValid()
        {
            var licString = await licenseStorage.FetchAsync();
            if (string.IsNullOrEmpty(licString))
            {
                var lic = Standard.Licensing.License.Load(licString);
                var publicKey = await publicKeyStorage.FetchAsync(lic.AdditionalAttributes.Get("PublicKeyName"));
                return !lic.Validate()
                    .ExpirationDate()
                    .And()
                    .Signature(publicKey)
                    .AssertValidLicense()
                    .Any();
            }

            return false;
        }
    }
}
