using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Xpo;

namespace Xenial.Licensing.Model
{
    [System.ComponentModel.Browsable(false)]
    public class UserLoginInfo : XenialLicenseBaseObjectId
    {
        private string loginProviderName;
        private string providerUserKey;
        private PermissionPolicyUser user;

        public UserLoginInfo(Session session) : base(session) { }

        public string LoginProviderName { get => loginProviderName; set => SetPropertyValue(ref loginProviderName, value); }

        public string ProviderUserKey { get => providerUserKey; set => SetPropertyValue(ref providerUserKey, value); }

        public PermissionPolicyUser User { get => user; set => SetPropertyValue(ref user, value); }
    }
}
