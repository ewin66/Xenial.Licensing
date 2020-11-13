using System;

using DevExpress.ExpressApp.Xpo;

namespace Xenial.Licensing.Blazor.Server.Services
{
    public class XpoDataStoreProviderAccessor
    {
        public IXpoDataStoreProvider DataStoreProvider { get; set; }
    }
}
