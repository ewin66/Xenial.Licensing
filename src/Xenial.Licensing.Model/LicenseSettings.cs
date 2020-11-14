using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

using Xenial.Licensing.Model.Infrastructure;

namespace Xenial.Licensing.Model
{
    [Persistent("LicenseSettings")]
    [Singleton]
    [DefaultClassOptions]
    [Appearance(nameof(LicenseSettings) + ".Delete", AppearanceItemType.Action, "1=1", TargetItems = "Delete")]
    [Appearance(nameof(LicenseSettings) + ".New", AppearanceItemType.Action, "1=1", TargetItems = "New;SaveAndNew")]
    public class LicenseSettings : XenialLicenseBaseObjectId
    {
        private LicensingKey defaultLicensingKey;
        private ProductBundle defaultProductBundle;
        private string uniqueId;

        public LicenseSettings(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            UniqueId = "6A566F71-F3EB-4AA5-A53D-FD61C76AF0E1";
        }

        [Indexed(Unique = true)]
        [System.ComponentModel.Browsable(false)]
        public string UniqueId { get => uniqueId; set => SetPropertyValue(ref uniqueId, value); }

        [Persistent("LicensingKeyId")]
        public LicensingKey DefaultLicensingKey { get => defaultLicensingKey; set => SetPropertyValue(ref defaultLicensingKey, value); }

        [Persistent("ProductBundleId")]
        public ProductBundle DefaultProductBundle { get => defaultProductBundle; set => SetPropertyValue(ref defaultProductBundle, value); }
    }
}
