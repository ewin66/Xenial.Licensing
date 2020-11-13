using System;
using System.ComponentModel;

using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xenial.Licensing.Model
{
    [Persistent("ProductBundle")]
    [DefaultClassOptions]
    public class ProductBundle : XenialLicenseBaseObjectId
    {
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductBundle"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public ProductBundle(Session session) : base(session) { }

        [Persistent("Name")]
        public string Name { get => name; set => SetPropertyValue(ref name, value); }

        [Association(nameof(ProductBundleItem.Bundle) + "-" + nameof(Items))]
        [Aggregated]
        public XPCollection<ProductBundleItem> Items => GetCollection<ProductBundleItem>();
    }

    [Persistent("ProductBundleItem")]
    public class ProductBundleItem : XenialLicenseBaseObjectId
    {
        private ProductBundle bundle;
        private string productName;
        private string productVersion;
        private Product product;
        private ProductVersion version;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductBundleItem"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public ProductBundleItem(Session session) : base(session) { }

        [Persistent("ProductBundleId")]
        [Association(nameof(Bundle) + "-" + nameof(ProductBundle.Items))]
        public ProductBundle Bundle { get => bundle; set => SetPropertyValue(ref bundle, value); }

        [Persistent("ProductName")]
        [Browsable(false)]
        public string ProductName { get => productName; set => SetPropertyValue(ref productName, value); }

        [Persistent("ProductVersion")]
        [Browsable(false)]
        public string ProductVersion { get => productVersion; set => SetPropertyValue(ref productVersion, value); }

        [Persistent("ProductId")]
        public Product Product { get => product; set => SetPropertyValue(ref product, value, p => ProductName = p?.Name); }

        [Persistent("ProductVersionId")]
        public ProductVersion Version { get => version; set => SetPropertyValue(ref version, value, v => ProductVersion = v?.Version); }

    }
}
