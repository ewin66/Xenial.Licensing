using System.ComponentModel;

using DevExpress.Xpo;

namespace Xenial.Licensing.Model
{
    [NonPersistent]
    public abstract class XenialLicenseBaseObjectId : XenialLicenseBaseObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XenialLicenseBaseObjectId"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public XenialLicenseBaseObjectId(Session session) : base(session) { }

        [Key(AutoGenerate = true)]
        [Persistent("Id")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Needed by XPO")]
        private int id = -1;
        [PersistentAlias(nameof(id))]
        public int Id => id;
    }
}
