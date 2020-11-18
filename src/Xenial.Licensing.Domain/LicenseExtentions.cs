using System.Linq;

using Standard.Licensing.Validation;

#nullable enable

namespace Xenial.Licensing.Domain
{
    public static class LicenseExtentions
    {
        public static bool HasExpired(this Standard.Licensing.License? license)
            => license == null || license.Validate()
                .ExpirationDate()
                .AssertValidLicense()
                .ToList()
                .Any();
    }
}
