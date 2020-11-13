using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.SystemModule;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DevExpress.ExpressApp.Xpo;
using Xenial.Licensing.Blazor.Server.Services;
using DevExpress.ExpressApp.Templates;
using Xenial.Licensing.Module.Blazor.Templates;

namespace Xenial.Licensing.Blazor.Server
{
    public partial class LicensingBlazorApplication : BlazorApplication
    {
        public LicensingBlazorApplication()
            => InitializeComponent();

        protected override void OnSetupStarted()
        {
            base.OnSetupStarted();
            var configuration = ServiceProvider.GetRequiredService<IConfiguration>();
            if (configuration.GetConnectionString("ConnectionString") != null)
            {
                ConnectionString = configuration.GetConnectionString("ConnectionString");
            }
#if EASYTEST
            if(configuration.GetConnectionString("EasyTestConnectionString") != null) {
                ConnectionString = configuration.GetConnectionString("EasyTestConnectionString");
            }
#endif
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached && CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema)
            {
                DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            }
#endif
        }
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            var dataStoreProvider = GetDataStoreProvider(args.ConnectionString, args.Connection);
            args.ObjectSpaceProviders.Add(new SecuredObjectSpaceProvider((ISelectDataSecurityProvider)Security, dataStoreProvider, true));
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }
        private IXpoDataStoreProvider GetDataStoreProvider(string connectionString, System.Data.IDbConnection connection)
        {
            var accessor = ServiceProvider.GetRequiredService<XpoDataStoreProviderAccessor>();
            lock (accessor)
            {
                if (accessor.DataStoreProvider == null)
                {
                    accessor.DataStoreProvider = XPObjectSpaceProvider.GetDataStoreProvider(connectionString, connection, true);
                }
            }
            return accessor.DataStoreProvider;
        }
        private void LicensingBlazorApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e)
        {
            e.Updater.Update();
            e.Handled = true;
        }

        protected override IFrameTemplate CreateDefaultTemplate(TemplateContext context)
        {
            if (context == TemplateContext.ApplicationWindow)
            {
                return new XenialWindowTemplate() { AboutInfoString = AboutInfo.Instance.GetAboutInfoString(this) };
            }

            return base.CreateDefaultTemplate(context);
        }
    }
}
