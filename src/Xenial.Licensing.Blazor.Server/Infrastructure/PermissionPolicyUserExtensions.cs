using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xenial.Licensing.Model;

namespace Xenial.Licensing.Blazor.Server.Infrastructure
{
    public static class PermissionPolicyUserExtensions
    {
        public static bool IsAuthenticationStandardEnabled(this PermissionPolicyUser _)
            => false;

        public static void CreateUserLoginInfo(this PermissionPolicyUser user, IObjectSpace os, string providerName, string providerUserKey)
        {
            var userLoginInfo = os.CreateObject<UserLoginInfo>();
            userLoginInfo.ProviderUserKey = providerUserKey;
            userLoginInfo.LoginProviderName = providerName;
            userLoginInfo.User = user;
            os.CommitChanges();
        }
    }
}
