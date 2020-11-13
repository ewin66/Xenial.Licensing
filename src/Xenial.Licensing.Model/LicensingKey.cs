using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xenial.Licensing.Model
{
    [Persistent("LicensingKey")]
    [DefaultClassOptions]
    public class LicensingKey : XenialLicenseBaseObjectId
    {
        private string name;
        private string passPhrase;
        private string privateKey;
        private string publicKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensingKey"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public LicensingKey(Session session) : base(session) { }

        [Persistent("Name")]
        public string Name { get => name; set => SetPropertyValue(ref name, value); }

        [Persistent("PassPhrase")]
        public string PassPhrase { get => passPhrase; set => SetPropertyValue(ref passPhrase, value); }

        [Persistent("PrivateKey")]
        [Size(SizeAttribute.Unlimited)]
        public string PrivateKey { get => privateKey; set => SetPropertyValue(ref privateKey, value); }

        [Persistent("PublicKey")]
        [Size(SizeAttribute.Unlimited)]
        public string PublicKey { get => publicKey; set => SetPropertyValue(ref publicKey, value); }
    }
}
