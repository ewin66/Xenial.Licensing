using System;
using System.ComponentModel;

using DevExpress.Xpo;

namespace Xenial.Licensing.Model
{
    [Persistent("ProductVersion")]
    [DefaultProperty(nameof(Version))]
    public class ProductVersion : XenialLicenseBaseObjectId
    {
        private string version;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVersion"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public ProductVersion(Session session) : base(session) { }

        [Persistent("Version")]
        public string Version { get => version; set => SetPropertyValue(ref version, value); }
    }
}
