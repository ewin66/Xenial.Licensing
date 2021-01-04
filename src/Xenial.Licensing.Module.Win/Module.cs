using System;

using DevExpress.ExpressApp;

namespace Xenial.Licensing.Module.Win
{
    public class LicensingWindowsFormsModule : ModuleBase
    {
        protected override ModuleTypeList GetRequiredModuleTypesCore()
            => base.GetRequiredModuleTypesCore()
                .AndModuleTypes(
                    typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule),
                    typeof(LicensingModule)
                );

    }
}
