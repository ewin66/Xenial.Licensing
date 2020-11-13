using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using AutoMapper.QueryableExtensions;

using DevExpress.Xpo;

using IdentityModel.Client;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Xenial.Licensing.Api.Mappers;
using Xenial.Licensing.Model;

namespace Xenial.Licensing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class LicensesController : ControllerBase
    {
        private readonly UnitOfWork unitOfWork;

        public LicensesController(DevExpress.Xpo.UnitOfWork unitOfWork)
            => this.unitOfWork = unitOfWork;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>OutLicenseModel</returns>
        [HttpGet]
        //[ProducesResponseType(typeof(string), 400)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(typeof(OutLicenseModel[]), 200)]
        [ProducesResponseType(typeof(OutLicenseModel), 200)]
        [Route("active")]
        public async Task<IActionResult> GetActive()
        {
            if (User.Identity is System.Security.Claims.ClaimsIdentity claimIdentity)
            {
                var idClaim = claimIdentity.FindFirst("sub");
                if (idClaim == null)
                {
                    return BadRequest("no sub claim");
                }
                var id = idClaim.Value;
                var licenses = await unitOfWork
                    .Query<License>()
                    .Where(l => l.User != null && l.User.Id == id)
                    .ProjectTo<OutLicenseModel>(LicenseMapper.Mapper.ConfigurationProvider)
                    .ToListAsync();

                return Ok(licenses);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("new/trial")]
        public async Task<IActionResult> RequestTrial()
            => await Task.FromResult(Ok());
    }

    public class OutLicenseModel
    {
        public string Id { get; set; }
    }
}
