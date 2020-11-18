using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

using System;
using System.Linq;

using Xenial.Licensing.Model.Infrastructure;

#nullable enable

namespace Xenial.Licensing.Model
{
    [Persistent("GrantedLicense")]
    [DefaultClassOptions]
    public class GrantedLicense : XenialLicenseBaseObject
    {
        private LicenseType type;
        private DateTime? expiresAt;
        private bool expiresNever;
        private int? maximumUtilization;
        private Standard.Licensing.License? generatedLicense;
        private LicensingKey? key;
        private CompanyUser? user;
        private ProductBundle? productBundle;
        private Guid id = Guid.NewGuid();
        private ProductVersion? productVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrantedLicense"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public GrantedLicense(Session session) : base(session)
            => session.PreFetch(Products);

        protected override void OnLoaded()
        {
            base.OnLoaded();
            //Cross thread operation
            var _ = Products.Count;
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            var settings = Session.FindObject<LicenseSettings>(null);
            Type = LicenseType.Trial;
            if (settings != null)
            {
                ExpiresAt = DateTime.UtcNow.AddDays(settings.DefaultTrialPeriod).Date;
                Key = settings.DefaultLicensingKey;
                MaximumUtilization = settings.DefaultMaximumUtilization;
                ProductBundle = settings.DefaultProductBundle;
            }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            var builder = Standard.Licensing.License.New()
                .As((Standard.Licensing.LicenseType)Type)
                .WithUniqueIdentifier(Id);

            if (ExpiresAt.HasValue)
            {
                builder.ExpiresAt(ExpiresAt.Value.Date);
            }

            if (MaximumUtilization.HasValue)
            {
                builder.WithMaximumUtilization(MaximumUtilization.Value);
            }

            if (User != null)
            {
                builder.LicensedTo(c =>
                {
                    c.Company = User.Company?.Name;
                    c.Name = User.Name;
                    c.Email = User.Email;
                });
            }

            builder.WithAdditionalAttributes(c =>
            {
                c.Add("CreatedAt", DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
                if (Key != null)
                {
                    c.Add("PublicKeyId", Key.Id.ToString());
                    c.Add("PublicKeyName", Key.Name);
                }

                if (User != null)
                {
                    c.Add(nameof(Company.CompanyGlobalId), User.Company?.CompanyGlobalId?.ToString());
                }

                if (ProductBundle != null)
                {
                    c.Add("ProductBundle", ProductBundle.Name);
                }
            });

            builder.WithProductFeatures(Products.ToDictionary(i => i.Product, _ => ProductVersion?.Version));

            if (Key != null)
            {
                GeneratedLicense = builder.CreateAndSignWithPrivateKey(Key.PrivateKey, Key.PassPhrase);
            }
        }

        public enum LicenseType
        {
            Trial = 1,
            Standard = 2,
        }

        [Persistent("Id")]
        [Key(AutoGenerate = false)]
        public Guid Id { get => id; set => SetPropertyValue(ref id, value); }

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
        public ProductBundle? ProductBundle
        {
            get => productBundle;
            set => SetPropertyValue(ref productBundle, value, product =>
            {
                foreach (var licenseProduct in Products.ToList())
                {
                    Products.Remove(licenseProduct);
                }

                if (product != null)
                {
                    foreach (var productBundle in product.Products)
                    {
                        Products.Add(new GrantedLicenseProduct(Session)
                        {
                            Product = productBundle.Product.Name
                        });
                    }
                }
            });
        }

        [Persistent("ProductVersionId")]
        public ProductVersion? ProductVersion { get => productVersion; set => SetPropertyValue(ref productVersion, value); }

        [Association(nameof(GrantedLicenseProduct.License) + "-" + nameof(Products))]
        [Aggregated]
        public XPCollection<GrantedLicenseProduct> Products => GetCollection<GrantedLicenseProduct>();
    }

#nullable disable

    [Persistent("GrantedLicenseProduct")]
    public class GrantedLicenseProduct : XenialLicenseBaseObjectId
    {
        private GrantedLicense license;
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrantedLicenseProduct"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public GrantedLicenseProduct(Session session) : base(session) { }

        [Persistent("LicenseId")]
        [Association(nameof(License) + "-" + nameof(GrantedLicense.Products))]
        public GrantedLicense License { get => license; set => SetPropertyValue(ref license, value); }

        [Persistent("Product")]
        public string Product { get => name; set => SetPropertyValue(ref name, value); }
    }
}
