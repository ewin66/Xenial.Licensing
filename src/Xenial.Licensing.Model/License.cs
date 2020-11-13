using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using System;

namespace Xenial.Licensing.Model
{

    [Persistent("License")]
    [DefaultClassOptions]
    public class License : XenialLicenseBaseObjectId
    {
        private LicenseType type;
        private DateTime expiresAt;
        private bool expiresNever;
        private int? maximumUtilization;
        private string generatedLicense;
        private LicensingKey key;
        private Company company;
        private CompanyUser user;
        private ProductBundle productBundle;

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
        public DateTime ExpiresAt { get => expiresAt; set => SetPropertyValue(ref expiresAt, value); }

        [Persistent("ExpiresNever")]
        public bool ExpiresNever { get => expiresNever; set => SetPropertyValue(ref expiresNever, value); }

        [Persistent("MaximumUtilization")]
        public int? MaximumUtilization { get => maximumUtilization; set => SetPropertyValue(ref maximumUtilization, value); }

        [Persistent("GeneratedLicense")]
        [Size(SizeAttribute.Unlimited)]
        public string GeneratedLicense { get => generatedLicense; set => SetPropertyValue(ref generatedLicense, value); }

        [Persistent("LicensingKeyId")]
        public LicensingKey Key { get => key; set => SetPropertyValue(ref key, value); }

        [Persistent("CompanyId")]
        public Company Company
        {
            get => company;
            set => SetPropertyValue(ref company, value, _ =>
            {
                User = null;
            });
        }

        [Persistent("CompanyUserId")]
        //[DataSourceProperty(nameof(Company) + "." + nameof(BusinessObjects.Company.Users))]
        public CompanyUser User { get => user; set => SetPropertyValue(ref user, value); }

        [Persistent("ProductBundleId")]
        public ProductBundle ProductBundle { get => productBundle; set => SetPropertyValue(ref productBundle, value); }

        private bool? isValid;
        [NonPersistent]
        public bool? IsValid { get => isValid; set => SetPropertyValue(ref isValid, value); }

        private string validationLog;
        [NonPersistent, Size(SizeAttribute.Unlimited)]
        public string ValidationLog { get => validationLog; set => SetPropertyValue(ref validationLog, value); }

        //[Action]
        //public void GenerateLic()
        //{
        //    ObjectSpace.CommitChanges();
        //    if (ExpiresNever) ExpiresAt = DateTime.MaxValue;

        //    var lic = New()
        //        .As((Standard.Licensing.LicenseType)Type)
        //        .ExpiresAt(ExpiresAt)
        //        .WithMaximumUtilization(MaximumUtilization.HasValue ? MaximumUtilization.Value : int.MaxValue)
        //        .LicensedTo(c =>
        //        {
        //            c.Company = Company.Name;
        //            c.Name = User.Name;
        //            c.Email = User.Email;
        //        })
        //        .WithAdditionalAttributes(c =>
        //        {
        //            c.Add("CreatedAt", DateTime.Now.ToString());
        //            c.Add("LicenseId", Oid.ToString());
        //            c.Add(nameof(Company.CompanyGlobalId), Company.CompanyGlobalId.ToString());
        //        })
        //        .WithUniqueIdentifier(ProductBundle.Oid)
        //        .WithProductFeatures(ProductBundle.Items.ToDictionary(i => i.ProductName, i => i.ProductVersion))
        //        .CreateAndSignWithPrivateKey(Key.PrivateKey, Key.PassPhrase);

        //    GeneratedLicense = lic.ToString();

        //    ObjectSpace.CommitChanges();
        //}

        //[Action]
        //public void Check()
        //{
        //    IsValid = null;
        //    ValidationLog = null;
        //    if (GeneratedLicense == null)
        //    {
        //        return;
        //    }
        //    var lic = Load(GeneratedLicense);


        //    var result = lic.Validate()
        //        .ExpirationDate()
        //        .And()
        //        .Signature(Key.PublicKey)
        //        .AssertValidLicense()
        //        .ToList();

        //    IsValid = !result.Any();

        //    var sb = new StringBuilder();
        //    foreach (var failure in result)
        //    {
        //        var msg = failure.GetType().Name + ": " + failure.Message + " - " + failure.HowToResolve;
        //        sb.AppendLine(msg);
        //        System.Diagnostics.Debug.WriteLine(msg);
        //    }
        //    ValidationLog = sb.ToString();
        //}
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
