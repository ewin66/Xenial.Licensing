using System;
using System.ComponentModel;

using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xenial.Licensing.Model
{
    [Persistent("ProductBundle")]
    [DefaultClassOptions]
    public class ProductBundle : XenialLicenseBaseObject
    {
        private string name;
        private string id = IdentityModel.CryptoRandom.CreateUniqueId();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductBundle"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public ProductBundle(Session session) : base(session) { }

        [Persistent("Id")]
        [Key(AutoGenerate = false)]
        [DevExpress.ExpressApp.Model.ModelDefault("AllowEdit", "False")]
        public string Id { get => id; set => SetPropertyValue(ref id, value); }

        [Persistent("Name")]
        public string Name { get => name; set => SetPropertyValue(ref name, value); }

        [Association(nameof(ProductBundleItem.Bundle) + "-" + nameof(Products))]
        [Aggregated]
        public XPCollection<ProductBundleItem> Products => GetCollection<ProductBundleItem>();
    }

    [Persistent("ProductBundleItem")]
    public class ProductBundleItem : XenialLicenseBaseObjectId
    {
        private ProductBundle bundle;
        private Product product;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductBundleItem"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public ProductBundleItem(Session session) : base(session) { }

        [Persistent("ProductBundleId")]
        [Association(nameof(Bundle) + "-" + nameof(ProductBundle.Products))]
        public ProductBundle Bundle { get => bundle; set => SetPropertyValue(ref bundle, value); }

        [Persistent("ProductId")]
        [DevExpress.Persistent.Validation.RuleRequiredField]
        public Product Product { get => product; set => SetPropertyValue(ref product, value); }

    }
}
