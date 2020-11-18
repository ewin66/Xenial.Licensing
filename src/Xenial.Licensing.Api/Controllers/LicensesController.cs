using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper.QueryableExtensions;

using DevExpress.Xpo;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Xenial.Licensing.Api.Mappers;
using Xenial.Licensing.Domain;
using Xenial.Licensing.Domain.Commands;
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

        public LicensesController(UnitOfWork unitOfWork)
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
                var expired = DateTime.UtcNow.Date;

                var licenses = await unitOfWork
                    .Query<GrantedLicense>()
                    .Where(l => l.User != null && l.User.Id == id && l.ExpiresNever || l.ExpiresAt >= expired)
                    .ProjectTo<OutLicenseModel>(LicenseMapper.Mapper.ConfigurationProvider)
                    .ToListAsync();

                licenses = licenses
                    .Where(l => !Standard.Licensing.License.Load(l.License).HasExpired())
                    .ToList();

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

                var settings = await unitOfWork.GetSingletonAsync<LicenseSettings>();

                ValidateSettings(settings);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = idClaim.Value;

                try
                {
                    var trialLicence = await new TrialRequestCommandHandler(unitOfWork)
                        .ExecuteAsync(new TrialRequestCommand(userId, model.MachineKey, null, null));

                    return Ok(new OutLicenseModel
                    {
                        Id = trialLicence.Id,
                        ExpiresAt = trialLicence.ExpiresAt,
                        License = trialLicence.License,
                        PublicKey = trialLicence.PublicKey
                    });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(nameof(TrialRequestCommandHandler), ex.Message);
                }
            }

            return BadRequest();

            void ValidateSettings(LicenseSettings settings)
            {
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
            }
        }
    }

    public class OutLicenseModel
    {
        public Guid Id { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string License { get; set; }
        public string PublicKey { get; set; }
    }

    public class InRequestTrialModel
    {
        [Required]
        public string MachineKey { get; set; }
    }
}
