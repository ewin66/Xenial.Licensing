using System;
using System.Collections.Generic;
using System.Linq;

namespace Xenial.Licensing.Model
{
    public static class LicensingModelTypeListExtentions
    {
        private static class ModelTypeList
        {
            internal static readonly IEnumerable<Type> ModelTypes = new[]
            {
                typeof(XenialLicenseBaseObject),
                typeof(XenialLicenseBaseObjectId),

                typeof(UserLoginInfo),

                typeof(Company),
                typeof(CompanyUser),
                typeof(License),
                typeof(LicensingKey),

                typeof(Product),
                typeof(ProductVersion),
                typeof(ProductBundle),
                typeof(ProductBundleItem),
            };
        }

        public static IEnumerable<Type> UseLicensingPersistentModels(this IEnumerable<Type> types)
            => types.Concat(ModelTypeList.ModelTypes);
    }
}
