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
    public class PublicKeyController : ControllerBase
    {
        private readonly UnitOfWork unitOfWork;

        public PublicKeyController(UnitOfWork unitOfWork)
            => this.unitOfWork = unitOfWork;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>OutKeyModel</returns>
        [HttpGet]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(SerializableError), 400)]
        [ProducesResponseType(typeof(OutKeyModel), 200)]
        [Route("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var key = await unitOfWork.GetObjectByKeyAsync<LicensingKey>(id);
            if (key != null)
            {
                return Ok(new OutKeyModel
                {
                    PublicKey = key.PublicKey,
                    Name = key.Name
                });
            }
            return NotFound(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>OutKeyModel</returns>
        [HttpGet]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(SerializableError), 400)]
        [ProducesResponseType(typeof(OutKeyModel), 200)]
        [Route("name/{name}")]
        public async Task<IActionResult> GetKeyByName(string name)
        {
            var key = await unitOfWork.Query<LicensingKey>().FirstOrDefaultAsync(k => k.Name == name);
            if (key != null)
            {
                return Ok(new OutKeyModel
                {
                    PublicKey = key.PublicKey,
                    Name = key.Name
                });
            }
            return NotFound(name);
        }
    }

    public class OutKeyModel
    {
        public string PublicKey { get; set; }
        public string Name { get; set; }
    }
}
