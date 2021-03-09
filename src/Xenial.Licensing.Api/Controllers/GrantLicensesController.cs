using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DevExpress.Xpo;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Xenial.Licensing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class GrantLicensesController : ControllerBase
    {
        private readonly UnitOfWork unitOfWork;

        public GrantLicensesController(UnitOfWork unitOfWork)
            => this.unitOfWork = unitOfWork;

    }
}
