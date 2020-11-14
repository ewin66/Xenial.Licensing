using System;
using System.Text;
using System.Linq;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Xpo;
using Xenial.Licensing.Model;
using Xenial.Licensing.Module.BusinessObjects;

namespace Xenial.Licensing.Module
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
    public sealed partial class LicensingModule : ModuleBase
    {
        public LicensingModule()
            => InitializeComponent();

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes()
            => base.GetDeclaredExportedTypes()
                .UseLicensingPersistentModels()
                .UseLicensingViewModels();

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.SetupComplete -= Application_SetupComplete;
            application.SetupComplete += Application_SetupComplete;
        }

        private void Application_SetupComplete(object sender, EventArgs e)
        {
            Application.ObjectSpaceCreated -= Application_ObjectSpaceCreated;
            Application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
        }

        private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e)
        {
            var objectSpace = e.ObjectSpace as CompositeObjectSpace;
            if (objectSpace != null)
            {
                if (!(objectSpace.Owner is CompositeObjectSpace))
                {
                    objectSpace.PopulateAdditionalObjectSpaces((XafApplication)sender);
                }
            }
        }
    }
}
