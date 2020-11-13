using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xenial.Licensing.Module.Blazor.Components
{
    public class ProfilePictureComponentBase : ComponentBase
    {
        [Inject]
        private IHttpContextAccessor HttpContextAccessor { get; set; }

        public string Inititals
            => HttpContextAccessor?
                .HttpContext?
                .User?
                .Claims.Where(c => c.Type == "xenial_initials")
                .Select(c => c.Value)
                .FirstOrDefault()
                ?? null;

        public string ForeColor
            => HttpContextAccessor?
                .HttpContext?
                .User?
                .Claims.Where(c => c.Type == "xenial_forecolor")
                .Select(c => c.Value)
                .FirstOrDefault()
                ?? null;

        public string BackColor
            => HttpContextAccessor?
                .HttpContext?
                .User?
                .Claims.Where(c => c.Type == "xenial_backcolor")
                .Select(c => c.Value)
                .FirstOrDefault()
                ?? null;

        public string ImageUri
            => HttpContextAccessor
                .HttpContext?
                .User?
                .Claims.Where(c => c.Type == "picture")
                .Select(c => c.Value)
                .FirstOrDefault()
                ?? null;

        [Parameter]
        public int Size { get; set; }

        [Parameter]
        public string Styles { get; set; }

        [Parameter]
        public bool Large { get; set; }
    }
}
