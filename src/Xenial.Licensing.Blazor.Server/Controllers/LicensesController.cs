using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Xenial.Licensing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class LicensesController : ControllerBase
    {

    }

    public class OutRequestTrialModel
    {

    }

    public class InRequestTrialModel
    {

    }
}
