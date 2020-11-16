using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper.QueryableExtensions;

using DevExpress.Xpo;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Xenial.Licensing.Api.Mappers;
using Xenial.Licensing.Model;
using Xenial.Licensing.Model.Infrastructure;

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
        [ProducesResponseType(typeof(SerializableError), 400)]
        [ProducesResponseType(typeof(OutLicenseModel[]), 200)]
        [Route("active")]
        public async Task<IActionResult> ListActive()
        {
            if (User.Identity is System.Security.Claims.ClaimsIdentity claimIdentity)
            {
                var idClaim = claimIdentity.FindFirst("sub");
                if (idClaim == null)
                {
                    ModelState.AddModelError("sub", "no sub claim");
                    return BadRequest(ModelState);
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
        [Route("request/trial")]
        [ProducesResponseType(typeof(SerializableError), 400)]
        [ProducesResponseType(typeof(OutLicenseModel), 200)]
        public async Task<IActionResult> RequestTrial(InRequestTrialModel model)
        {
            if (User.Identity is System.Security.Claims.ClaimsIdentity claimIdentity)
            {
                var idClaim = claimIdentity.FindFirst("sub");
                if (idClaim == null)
                {
                    ModelState.AddModelError("sub", "no sub claim");
                    return BadRequest(ModelState);
                }
                var userId = idClaim.Value;
                var trialRequest = new TrialRequest(unitOfWork)
                {
                    MachineKey = model.MachineKey,
                    UserId = userId
                };

                await unitOfWork.SaveAsync(trialRequest);
                await unitOfWork.CommitChangesAsync();

                var settings = await unitOfWork.GetSingletonAsync<LicenseSettings>();
                if (settings == null)
                {
                    ModelState.AddModelError(nameof(LicenseSettings), $"{nameof(LicenseSettings)} not found");
                }
                else
                {
                    if (settings.DefaultLicensingKey == null)
                    {
                        ModelState.AddModelError(nameof(LicenseSettings), $"{nameof(LicenseSettings)}.{nameof(LicenseSettings.DefaultLicensingKey)} not set");
                    }
                    if (settings.DefaultProductBundle == null)
                    {
                        ModelState.AddModelError(nameof(LicenseSettings), $"{nameof(LicenseSettings)}.{nameof(LicenseSettings.DefaultProductBundle)} not set");
                    }
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var license = new License(unitOfWork)
                {
                    User = await unitOfWork.GetObjectByKeyAsync<CompanyUser>(userId),
                };

                unitOfWork.Save(license);
                unitOfWork.CommitChanges();

                return Ok(new OutLicenseModel
                {
                    Id = license.Id,
                    ExpiresAt = license.ExpiresAt.Value,
                    License = license.GeneratedLicense.ToString(),
                });
            }
            return BadRequest();
        }
    }

    public class OutLicenseModel
    {
        public Guid Id { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string License { get; set; }
    }

    public class InRequestTrialModel
    {
        [Required]
        public string MachineKey { get; set; }
    }
}
