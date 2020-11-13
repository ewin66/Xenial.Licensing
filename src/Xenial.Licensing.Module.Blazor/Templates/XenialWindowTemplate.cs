using DevExpress.ExpressApp.Blazor.Templates;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Text;

using Xenial.Licensing.Module.Blazor.Components;

namespace Xenial.Licensing.Module.Blazor.Templates
{
    public class XenialWindowTemplate : WindowTemplate
    {
        protected override RenderFragment CreateControl()
            => XenialWindowTemplateComponent.Creator(this);
    }
}
