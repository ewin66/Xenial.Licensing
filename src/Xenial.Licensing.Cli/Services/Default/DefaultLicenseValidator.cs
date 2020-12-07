using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Standard.Licensing.Validation;

#nullable enable

namespace Xenial.Licensing.Cli.Services.Default
{
    public class DefaultLicenseValidator : ILicenseValidator
    {
        private readonly ILicensePublicKeyStorage publicKeyStorage;

        public DefaultLicenseValidator(ILicensePublicKeyStorage publicKeyStorage)
            => this.publicKeyStorage = publicKeyStorage;

        public async Task<bool> IsValid(string licString, string? publicKey = null)
        {
            if (!string.IsNullOrEmpty(licString))
            {
                var lic = Standard.Licensing.License.Load(licString);

                if (string.IsNullOrEmpty(publicKey))
                {
                    publicKey = await publicKeyStorage.FetchAsync(lic.AdditionalAttributes.Get("PublicKeyName"));
                }

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
