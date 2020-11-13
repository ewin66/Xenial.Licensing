using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

using System;

using Xenial.Licensing.Model.Infrastructure;

#nullable enable

namespace Xenial.Licensing.Model
{

    [Persistent("License")]
    [DefaultClassOptions]
    public class License : XenialLicenseBaseObjectId
    {
        private LicenseType type;
        private DateTime? expiresAt;
        private bool expiresNever;
        private int? maximumUtilization;
        private Standard.Licensing.License? generatedLicense;
        private LicensingKey? key;
        private CompanyUser? user;
        private ProductBundle? productBundle;

        /// <summary>
        /// Initializes a new instance of the <see cref="License"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public License(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            ExpiresAt = DateTime.Now;
            Type = LicenseType.Trial;
        }

        public enum LicenseType
        {
            Trial = 1,
            Standard = 2,
        }

        [Persistent("Type")]
        public LicenseType Type { get => type; set => SetPropertyValue(ref type, value); }

        [Persistent("ExpiresAt")]
        [ValueConverter(typeof(UtcDateTimeConverter))]
        public DateTime? ExpiresAt { get => expiresAt; set => SetPropertyValue(ref expiresAt, value); }

        [Persistent("ExpiresNever")]
        public bool ExpiresNever { get => expiresNever; set => SetPropertyValue(ref expiresNever, value); }

        [Persistent("MaximumUtilization")]
        public int? MaximumUtilization { get => maximumUtilization; set => SetPropertyValue(ref maximumUtilization, value); }

        [Persistent("GeneratedLicense")]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(LicenseValueConverter))]
        public Standard.Licensing.License? GeneratedLicense { get => generatedLicense; set => SetPropertyValue(ref generatedLicense, value); }

        [Persistent("LicensingKeyId")]
        public LicensingKey? Key { get => key; set => SetPropertyValue(ref key, value); }

        [Persistent("CompanyUserId")]
        public CompanyUser? User { get => user; set => SetPropertyValue(ref user, value); }

        [Persistent("ProductBundleId")]
        public ProductBundle? ProductBundle { get => productBundle; set => SetPropertyValue(ref productBundle, value); }
    }

    [Persistent("LicenseProduct")]
    public class LicenseProduct : XenialLicenseBaseObjectId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseProduct"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public LicenseProduct(Session session) : base(session) { }
    }
}
